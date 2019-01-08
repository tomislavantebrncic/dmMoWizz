﻿using dmMoWizz.Models.Recommendations;
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

        public RecommendationService()
        {
            _moviesRepository = new MoviesRepository();
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

        public List<Recommendation> GetRecommendations(Info info)
        {
            #region Similars and myLikes
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

                    foreach (var hit in movieInfo.similar.results)
                    {
                        var similarMovieInfo = _moviesRepository.GetMovieFromTitle(hit.title);
                        if (similarMovieInfo != null)
                        {
                            var recommendation = new Recommendation
                            {
                                Movie = similarMovieInfo,
                                Rating = 1
                            };

                            var recommendedMovie = similars.FirstOrDefault(r => r.Equals(recommendation));
                            if (recommendedMovie == null)
                            {
                                similars.Add(recommendation);
                            }
                            else
                            {
                                recommendedMovie.Update(1);
                            }
                        }
                    }
                }
            }
            #endregion

            List<Recommendation> friendsSimilars = new List<Recommendation>();
            foreach (var friend in info.Friends.Data)
            {
                foreach (var movie in friend.Likes.Data)
                {
                    var movieInfo = _moviesRepository.GetMovieFromTitle(movie.Name);
                    if (movieInfo != null)
                    {
                        var recommendation = new Recommendation
                        {
                            Movie = movieInfo,
                            Rating = 1
                        };

                        var recommendedMovie = friendsSimilars.FirstOrDefault(r => r.Equals(recommendation));
                        if (recommendedMovie == null)
                        {
                            friendsSimilars.Add(recommendation);
                        } else
                        {
                            recommendedMovie.Update(1);
                        }
                    }
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

            return similars;
        }
    }
}