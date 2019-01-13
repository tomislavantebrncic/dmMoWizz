using dmMoWizz.Models.Mongo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace dmMoWizz.Services
{
    public class Downloader
    {   
        public static MovieInfo GetAdditionalData(MovieInfo movieDetails)
        {
            Tunefind tf = new Tunefind();
            TraktTV ttv = new TraktTV();
            List<TraktTVSimilar> traktTVSimilars = new List<TraktTVSimilar>();
            var ttvs = ttv.GetTraktTVResponse(movieDetails.imdb_id);
            if (ttvs != null)
            {
                foreach (var ts in ttv.GetTraktTVResponse(movieDetails.imdb_id))
                {
                    traktTVSimilars.Add(new TraktTVSimilar
                    {
                        Title = ts.title,
                        ImdbId = ts.ids.imdb,
                        TmdbId = ts.ids.tmdb
                    });
                }
            }
            movieDetails.TraktSimilars = traktTVSimilars;
            movieDetails.Ratings = GetRatingsFromImdbId(movieDetails.imdb_id);
            movieDetails.Soundtrack = GetSoundtrackFromTunefind(movieDetails.id, tf);
            movieDetails.AppRating = new AppRating();

            return movieDetails;
        }

        private static List<Rating> GetRatingsFromImdbId(string imdbId)
        {
            var apiRequest = WebRequest.Create("http://www.omdbapi.com/?apikey=" + "2fc4648d" + "&i=" + imdbId);

            var apiResponse = CallApi(apiRequest);
            if (apiResponse != null)
            {
                return JsonConvert.DeserializeObject<OMDBMovieInfo>(apiResponse).Ratings;
            }

            return new List<Rating>();
        }

        private class OMDBMovieInfo
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Rated { get; set; }
            public string Released { get; set; }
            public string Runtime { get; set; }
            public string Genre { get; set; }
            public string Director { get; set; }
            public string Writer { get; set; }
            public string Actors { get; set; }
            public string Plot { get; set; }
            public string Language { get; set; }
            public string Country { get; set; }
            public string Awards { get; set; }
            public string Poster { get; set; }
            public List<Rating> Ratings { get; set; }
            public string Metascore { get; set; }
            public string imdbRating { get; set; }
            public string imdbVotes { get; set; }
            public string imdbID { get; set; }
            public string Type { get; set; }
            public string DVD { get; set; }
            public string BoxOffice { get; set; }
            public string Production { get; set; }
            public string Website { get; set; }
            public string Response { get; set; }
        }

        private static string CallApi(WebRequest apiRequest)
        {
            try
            {
                using (var response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                return null;
            }
        }

        public class Ids
        {
            public int trakt { get; set; }
            public string slug { get; set; }
            public string imdb { get; set; }
            public int tmdb { get; set; }
        }

        public class TraktTVResponse
        {
            public string title { get; set; }
            public int year { get; set; }
            public Ids ids { get; set; }
        }

        public class Artist
        {
            public string id { get; set; }
            public string name { get; set; }
            public DateTime date_updated { get; set; }
            public string tunefind_url { get; set; }
            public string tunefind_api_url { get; set; }
        }

        public class Store
        {
            public string id { get; set; }
            public List<string> countries { get; set; }
            public string url { get; set; }
        }

        public class Appearance
        {
            public int song_id { get; set; }
            public string song_name { get; set; }
            public string artist_id { get; set; }
            public string artist_name { get; set; }
            public DateTime date_created { get; set; }
            public DateTime air_date { get; set; }
            public DateTime syndication_date { get; set; }
        }

        public class Song
        {
            public int id { get; set; }
            public string name { get; set; }
            public DateTime date_updated { get; set; }
            public string tunefind_url { get; set; }
            public string tunefind_api_url { get; set; }
            public Artist artist { get; set; }
            public List<Store> stores { get; set; }
            public Appearance appearance { get; set; }
        }

        public class TunefindResponse
        {
            public string id { get; set; }
            public string name { get; set; }
            public string tunefind_url { get; set; }
            public string tunefind_api_url { get; set; }
            public DateTime air_date { get; set; }
            public List<Song> songs { get; set; }
        }

        public class Tunefind
        {
            public TunefindResponse GetSoundtracks(int tmdbId)
            {
                var apiRequest = WebRequest.Create("https://8576d087.api.tunefind.com/api/v2/movie/" + tmdbId + "?id-type=tmdb");
                string username = "8576d08726a03a9b15188dccda93f40d";
                string password = "442014c5446f25621a9c92ace89f61c2";
                string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                apiRequest.Headers.Add("Authorization", "Basic " + encoded);

                var apiResponse = CallApi(apiRequest);

                if (apiResponse != null)
                {
                    return JsonConvert.DeserializeObject<TunefindResponse>(apiResponse);
                }

                return null;
            }
        }

        public class TraktTV
        {
            public List<TraktTVResponse> GetTraktTVResponse(string imdbId)
            {
                var apiRequest = WebRequest.Create("https://api.trakt.tv/movies/" + imdbId + "/related");
                apiRequest.Headers["trakt-api-key"] = "da9d164c7e5fb1d8b7039b11fbaa4f4ea6fed3175f326e98a085f4e3ddd7506e";
                apiRequest.Headers["trakt-api.version"] = "2";
                apiRequest.ContentType = "application/json";

                var apiResponse = CallApi(apiRequest);

                if (apiResponse != null)
                {
                    return JsonConvert.DeserializeObject<List<TraktTVResponse>>(apiResponse);
                }

                return null;

            }
        }

        private static List<dmMoWizz.Models.Mongo.Song> GetSoundtrackFromTunefind(int id, Tunefind tf)
        {
            List<dmMoWizz.Models.Mongo.Song> soundtrack = new List<dmMoWizz.Models.Mongo.Song>();
            var response = tf.GetSoundtracks(id);

            if (response != null)
            {
                foreach (var song in response.songs)
                {
                    soundtrack.Add(new dmMoWizz.Models.Mongo.Song
                    {
                        Id = song.id,
                        Name = song.name,
                        TunefindUrl = song.tunefind_url,
                        Artist = new dmMoWizz.Models.Mongo.Artist
                        {
                            Id = song.artist.id,
                            Name = song.artist.name,
                            TunefindUrl = song.artist.tunefind_url
                        }
                    });
                }
            }

            return soundtrack;
        }
    }
}