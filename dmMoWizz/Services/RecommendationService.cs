using dmMoWizz.Models;
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

        private static Info facebookInfo;

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

        public async Task<List<Recommendation>> GetRecommendations(UserInfo user, IList<System.Security.Claims.Claim> currentClaims, int friendsConst, int similarConst, int rateConst)
        {
            var info = await GetInfoAsync(currentClaims, "https://graph.facebook.com/me?fields=id,name,likes.limit(100){{category,name}},friends.limit(1000){{likes.limit(1000){{category,name}}}}&access_token={0}");

            facebookInfo = info;

            return GetRecommendations(user, info, friendsConst, similarConst, rateConst);
        }

        public async Task<List<Recommendation>> GetRecommendationsFriends(IList<System.Security.Claims.Claim> currentClaims)
        {
            return GetFriendsRecommendations(await GetInfoAsync(currentClaims, "https://graph.facebook.com/me?fields=id,name,likes.limit(100){{category,name}},friends.limit(1000){{likes.limit(1000){{category,name}}}}&access_token={0}"));
        }

        public List<Recommendation> GetRecommendations(UserInfo user, int friendsConst, int similarConst, int rateConst)
        {
            return GetRecommendations(user, facebookInfo, friendsConst, similarConst, rateConst);
        }


        public List<Recommendation> GetRecommendations(UserInfo user, Info info, int friendsConst, int similarConst, int rateConst)
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

                    FillSimilars(similars, movieInfo, similarConst);
                }
            }

            List<Recommendation> friendsSimilars = new List<Recommendation>();
            foreach (var friend in info.Friends.Data)
            {
                foreach (var movie in friend.Likes.Data)
                {
                    var movieInfo = _moviesRepository.GetMovieFromTitle(movie.Name);
                    UpdateRecommendationsForMovie(friendsSimilars, movieInfo, friendsConst);
                }
            }

            List<Recommendation> watchlistSimilars = new List<Recommendation>();
            foreach (var movie in user.Watchlist)
            {
                var movieInfo = _moviesRepository.GetMovie(movie.Id);
                FillSimilars(watchlistSimilars, movieInfo, similarConst);
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

            foreach (var recommendation in watchlistSimilars)
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

            foreach (var similar in similars)
            {
                var avgRate = CalculateAvgRate(similar.Movie);
                similar.Update(similar.Rating * avgRate * rateConst);
            }

            similars = similars.FindAll(r => !user.Watchlist.Contains(new WatchlistMovie { Id = r.Movie.id }));
            similars.Sort((rec1, rec2) => rec1.CompareTo(rec2));

            recommendations = similars;
            double max = similars.FirstOrDefault().Rating;
            foreach (var recommendation in similars)
            {
                recommendation.Rating = Math.Round(recommendation.Rating / max * 100, MidpointRounding.AwayFromZero);
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

                FillSimilars(similars, movieInfo, 5);
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
        private void FillSimilars(List<Recommendation> similars, MovieInfo movieInfo, int startingValue)
        {
            double val = startingValue;
            double step = val / 40;
            foreach (var hit in movieInfo.similar.results)
            {
                var similarMovieInfo = _moviesRepository.GetMovieFromTitle(hit.title);
                UpdateRecommendationsForMovie(similars, similarMovieInfo, val);
                val -= step;
            }

            val = startingValue;
            foreach (var hit in movieInfo.TraktSimilars)
            {
                var similarMovieInfo = _moviesRepository.GetMovie(hit.TmdbId);
                UpdateRecommendationsForMovie(similars, similarMovieInfo, val);
                val -= step;
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

        private double CalculateAvgRate(MovieInfo movie)
        {
            var ratings = movie.Ratings;
            double sum = 0;
            int count = 0;
            foreach (var rating in ratings)
            {
                var rate = rating.Value;
                if (rating.Source == "Internet Movie Database")
                {
                    count++;
                    var splitRate = rate.Split('/');
                    sum += double.Parse(splitRate[0], System.Globalization.CultureInfo.InvariantCulture) /
                        double.Parse(splitRate[1], System.Globalization.CultureInfo.InvariantCulture);

                } 
                else if (rating.Source == "Rotten Tomatoes")
                {
                    count++;
                    var splitRate = rate.Split('%');
                    sum += double.Parse(splitRate[0], System.Globalization.CultureInfo.InvariantCulture) / 100;
                }
                else if (rating.Source == "Metacritic")
                {
                    count++;
                    var splitRate = rate.Split('/');
                    sum += double.Parse(splitRate[0], System.Globalization.CultureInfo.InvariantCulture) /
                        double.Parse(splitRate[1], System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return sum / count;
        }
        #endregion
    }
}