using dmMoWizz.Models.Mongo;
using dmMoWizz.Models.TMDB;
using dmMoWizz.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace dmMoWizz.Services
{
    public class MovieService
    {
        private static readonly string _tmdbApiKey = "4c769c0e240a8d828da99a1019da67a8";

        private readonly MoviesRepository _movieRepository;

        public MovieService()
        {
            _movieRepository = new MoviesRepository();
        }

        public List<MovieInfo> GetPopular(int n)
        {
            string apiResponse = CallApi("https://api.themoviedb.org/3/movie/popular?page=1&language=en-US&api_key=" + _tmdbApiKey);
            TMDBPopularMoviesResponse popular = JsonConvert.DeserializeObject<TMDBPopularMoviesResponse>(apiResponse);

            List<MovieInfo> movies = popular.results.Take(n).Select(m => FetchMovieDetailsById(m.id)).ToList();
            movies.ForEach(m => UpdateDatabase(m));

            return movies;
        }

        private MovieInfo FetchMovieDetailsById(int id)
        {
            var apiCall = "https://api.themoviedb.org/3/movie/" + id + "?api_key=" + _tmdbApiKey + "&append_to_response=keywords,credits,similar,images";

            var apiResponse = CallApi(apiCall);
            if (apiResponse != null)
            {
                return JsonConvert.DeserializeObject<MovieInfo>(apiResponse);
            }

            return null;
        }

        private string CallApi(string apiCall)
        {
            var apiRequest = WebRequest.Create(apiCall);

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

        private void UpdateDatabase(MovieInfo movie)
        {
            if (_movieRepository.GetMovie(movie.id) == null)
            {
                _movieRepository.Insert(movie);
                System.Diagnostics.Debug.WriteLine("Inserting " + movie.title);
            }
        }
    }
}