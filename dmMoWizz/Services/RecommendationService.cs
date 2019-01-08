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

        public async Task<List<KeyValuePair<string, int>>> GetRecommendations(IList<System.Security.Claims.Claim> currentClaims)
        {
            return GetRecommendations(await GetInfoAsync(currentClaims, "https://graph.facebook.com/me?fields=id,name,likes.limit(100){{category,name}},friends.limit(1000){{likes.limit(1000){{category,name}}}}&access_token={0}"));
        }

        public List<KeyValuePair<string, int>> GetRecommendations(Info info)
        {
            ConcurrentDictionary<string, int> similars = new ConcurrentDictionary<string, int>();
            foreach (var movie in info.Likes.Data)
            {
                var movieInfo = _moviesRepository.GetMovieFromTitle(movie.Name);
                if (movieInfo != null)
                {
                    System.Diagnostics.Debug.WriteLine(movieInfo.title);
                    foreach (var hit in movieInfo.similar.results)
                    {
                        var t = hit.title;
                        similars.AddOrUpdate(t, 1, (id, count) => count + 1);
                    }
                }
            }

            ConcurrentDictionary<string, int> friendsSimilars = new ConcurrentDictionary<string, int>();
            foreach (var friend in info.Friends.Data)
            {
                foreach (var movie in friend.Likes.Data)
                {
                    var t = movie.Name;
                    friendsSimilars.AddOrUpdate(t, 1, (id, count) => count + 1);
                }
            }

            foreach (var pair in friendsSimilars)
            {
                similars.AddOrUpdate(pair.Key, pair.Value, (id, count) => count + pair.Value * 2);
            }

            var sorted = similars.ToList();
            sorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            return sorted;
        }
    }
}