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
        private IMongoCollection<MovieInfo> _moviesCollection;

        public MoviesRepository()
        {
            MongoClient client = new MongoClient(ConfigurationManager.AppSettings["FacebookAppId"]);
            IMongoDatabase db = client.GetDatabase("dm-mowizz");

            _moviesCollection = db.GetCollection<MovieInfo>("movies");
        }
    }
}