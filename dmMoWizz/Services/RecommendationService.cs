using dmMoWizz.Models.Mongo;
using dmMoWizz.Models.Recommendations;
using dmMoWizz.Models.SocialMedia.Facebook;
using dmMoWizz.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace dmMoWizz.Services
{
    public class RecommendationService
    {
        private readonly MoviesRepository _moviesRepository;
        private static readonly Dictionary<int, double> _scores;
        private static List<Recommendation> recommendations;

        static RecommendationService()
        {
            _scores = new Dictionary<int, double>();
            recommendations = new List<Recommendation>();
        }

        public RecommendationService()
        {
            _moviesRepository = new MoviesRepository();
        }

        public double GetRecommendation(int movieId)
        {
            System.Diagnostics.Debug.WriteLine("Get: " + _scores.Count);
            if (_scores.ContainsKey(movieId))
            {
                return _scores[movieId];
            }
            return -1;
        }

        public List<Recommendation> GetRecommendationList()
        {
            return recommendations;
        }

        public async Task<Info> GetInfoAsync(IList<System.Security.Claims.Claim> currentClaims, string urlString)
        {
            var accessToken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook");

            if (accessToken == null)
            {
                return null;
            }

            string url = String.Format(
                urlString, accessToken.Value);

            System.Diagnostics.Debug.WriteLine(url);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string result = await reader.ReadToEndAsync();

                dynamic jsonObj = System.Web.Helpers.Json.Decode(result);

                return new Info(jsonObj);
            }
        }

        public async Task<List<Recommendation>> GetRecommendations(IList<System.Security.Claims.Claim> currentClaims)
        {
            return GetRecommendations(await GetInfoAsync(currentClaims, "https://graph.facebook.com/me?fields=id,name,likes.limit(100){{category,name}},friends.limit(1000){{likes.limit(1000){{category,name}}}}&access_token={0}"));
        }

        public async Task<List<Recommendation>> GetRecommendationsFriends(IList<System.Security.Claims.Claim> currentClaims)
        {
            return GetFriendsRecommendations(await GetInfoAsync(currentClaims, "https://graph.facebook.com/me?fields=id,name,likes.limit(100){{category,name}},friends.limit(1000){{likes.limit(1000){{category,name}}}}&access_token={0}"));
        }


        public List<Recommendation> GetRecommendations(Info info)
        {
            List<Recommendation> similars = new List<Recommendation>();
            List<Recommendation> myLikes = new List<Recommendation>();
            foreach (var movie in info.Likes.Data)
            {
                var movieInfo = _moviesRepository.GetMovieFromTitle(movie.Name);
                if (movieInfo != null)
                {
                    myLikes.Add(new Recommendation
                    {
                        Movie = movieInfo,
                        Rating = 1
                    });

                    FillSimilars(similars, movieInfo);
                }
            }

            List<Recommendation> friendsSimilars = new List<Recommendation>();
            foreach (var friend in info.Friends.Data)
            {
                foreach (var movie in friend.Likes.Data)
                {
                    var movieInfo = _moviesRepository.GetMovieFromTitle(movie.Name);
                    UpdateRecommendationsForMovie(friendsSimilars, movieInfo, 1);
                }
            }

            foreach (var recommendation in friendsSimilars)
            {
                var recMy = myLikes.FirstOrDefault(r => r.Equals(recommendation));
                if (recMy != null)
                {
                    recMy.Update(recommendation.Rating * 2);
                }

                var recSim = similars.FirstOrDefault(r => r.Equals(recommendation));
                if (recSim != null)
                {
                    recSim.Update(recommendation.Rating * 2);
                }
            }

            foreach (var recommendation in myLikes)
            {
                var recSim = similars.FirstOrDefault(r => r.Equals(recommendation));
                if (recSim != null)
                {
                    recSim.Update(recommendation.Rating);
                }
                else
                {
                    similars.Add(recommendation);
                }
            }

            similars.Sort((rec1, rec2) => rec1.CompareTo(rec2));

            recommendations = similars;
            double max = similars.FirstOrDefault().Rating;
            foreach (var recommendation in similars)
            {
                recommendation.Rating = Math.Round(recommendation.Rating / max * 100, 2);
                _scores[recommendation.Movie.id] = recommendation.Rating;
            }

            return similars;
        }

        public List<Recommendation> GetWatchlistRecommendations(UserInfo userInfo)
        {
            List<Recommendation> similars = new List<Recommendation>();

            foreach (var watchlistMovie in userInfo.Watchlist)
            {
                var movieInfo = _moviesRepository.GetMovie(watchlistMovie.Id);

                FillSimilars(similars, movieInfo);
            }

            similars.Sort((rec1, rec2) => rec1.CompareTo(rec2));

            return similars;
        }

        public List<Recommendation> GetFriendsRecommendations(Info info)
        {
            List<Recommendation> recommendations = new List<Recommendation>();

            List<Recommendation> friendsSimilars = new List<Recommendation>();
            foreach (var friend in info.Friends.Data)
            {
                foreach (var movie in friend.Likes.Data)
                {
                    var movieInfo = _moviesRepository.GetMovieFromTitle(movie.Name);
                    UpdateRecommendationsForMovie(friendsSimilars, movieInfo, 1);
                }
            }

            return recommendations;
        }

        #region Helper Methods
        private void FillSimilars(List<Recommendation> similars, MovieInfo movieInfo)
        {
            double val = 5;
            foreach (var hit in movieInfo.similar.results)
            {
                var similarMovieInfo = _moviesRepository.GetMovieFromTitle(hit.title);
                UpdateRecommendationsForMovie(similars, similarMovieInfo, val);
                val -= 0.25;
            }
        }

        private void UpdateRecommendationsForMovie(List<Recommendation> recommendations, MovieInfo movieInfo, double val)
        {
            if (movieInfo != null)
            {
                var recommendation = new Recommendation
                {
                    Movie = movieInfo,
                    Rating = val
                };

                var recommendedMovie = recommendations.FirstOrDefault(r => r.Equals(recommendation));
                if (recommendedMovie == null)
                {
                    recommendations.Add(recommendation);
                }
                else
                {
                    recommendedMovie.Update(val);
                }
            }
        }
        #endregion
    }
}