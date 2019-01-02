using dmMoWizz.Models.Mongo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace dmMoWizz.Repositories
{
    public class MoviesRepository
    {
        private readonly IMongoCollection<MovieInfo> _moviesCollection;

        public MoviesRepository()
        {
            MongoClient client = new MongoClient(ConfigurationManager.AppSettings["FacebookAppId"]);
            IMongoDatabase db = client.GetDatabase("dm-mowizz");

            _moviesCollection = db.GetCollection<MovieInfo>("movies");
        }

        public MovieInfo GetMovie(int id)
        {
            return _moviesCollection.Find(m => m.id == id).FirstOrDefault();
        }

        public MovieInfo GetMovieFromImdbId(string imdbId)
        {
            return _moviesCollection.Find(m => m.imdb_id.Equals(imdbId)).FirstOrDefault();
        }

        public MovieInfo GetMovieFromTitle(string title)
        {
            return _moviesCollection.Find(m => m.title.Equals(title)).FirstOrDefault();
        }
    }
}