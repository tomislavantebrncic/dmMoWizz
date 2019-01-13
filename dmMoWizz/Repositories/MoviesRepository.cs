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
        private readonly IMongoCollection<Genre> _genres;

        public MoviesRepository()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = client.GetDatabase("dm-mowizz");

            _moviesCollection = db.GetCollection<MovieInfo>("movies");
            _genres = db.GetCollection<Genre>("genres");
        }

        public List<MovieInfo> GetMovies()
        {
            return _moviesCollection.Find(m => true).ToList();
        }

        public List<Genre> GetGenres()
        {
            return _genres.Find(g => true).ToList();
        }

        public List<MovieInfo> GetMovies(string titleRegex)
        {
            var filter = Builders<MovieInfo>.Filter.Regex(m => m.title, new MongoDB.Bson.BsonRegularExpression(titleRegex, "i"));
            return _moviesCollection.Find(filter).ToList();
        }

        public List<MovieInfo> GetMovies(string titleRegex, string genre, string year)
        {
            var builder = Builders<MovieInfo>.Filter;

            IList<FilterDefinition<MovieInfo>> filters = new List<FilterDefinition<MovieInfo>>();

            if (!String.IsNullOrEmpty(titleRegex))
            {
                filters.Add(builder.Regex(m => m.title, new MongoDB.Bson.BsonRegularExpression(titleRegex, "i")));
            }

            if (!year.Equals("All"))
            {
                filters.Add(builder.Regex(m => m.release_date, new MongoDB.Bson.BsonRegularExpression(".*" + year + ".*")));
            }

            if (!genre.Equals("All"))
            {
                filters.Add(builder.ElemMatch(m => m.genres, genre));
            }

            return _moviesCollection.Find(builder.And(filters)).ToList();
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
            var filter = Builders<MovieInfo>.Filter.Regex(m => m.title, new MongoDB.Bson.BsonRegularExpression(title, "i"));

            return _moviesCollection.Find(filter).FirstOrDefault();
        }

        public List<MovieInfo> GetMovies(string titleRegex, List<Genre> genres, string year)
        {
            var builder = Builders<MovieInfo>.Filter;

            IList<FilterDefinition<MovieInfo>> filters = new List<FilterDefinition<MovieInfo>>();

            if (!String.IsNullOrEmpty(titleRegex))
            {
                filters.Add(builder.Regex(m => m.title, new MongoDB.Bson.BsonRegularExpression(titleRegex, "i")));
            } else
            {
                filters.Add(builder.Regex(m => m.title, new MongoDB.Bson.BsonRegularExpression(".*")));
            }

            if (!year.Equals("All"))
            {
                filters.Add(builder.Regex(m => m.release_date, new MongoDB.Bson.BsonRegularExpression(".*" + year + ".*")));
            }

            if (genres.Count != 0)
            {
                foreach (var gen in genres)
                {
                    filters.Add(builder.ElemMatch<Genre>(m => m.genres, g => g.id == gen.id));
                }
            }

            return _moviesCollection.Find(builder.And(filters)).ToList();
        }

        public void UpdateRating(int movieId, int incRating, int incCount)
        {
            

            var filter = Builders<MovieInfo>.Filter.Eq(m => m.id, movieId);
            var update = Builders<MovieInfo>.Update.Inc(m => m.AppRating.Count, incCount).Inc(m => m.AppRating.Rating, incRating);

            try
            {
                _moviesCollection.UpdateOne(filter, update);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void Insert(MovieInfo movie)
        {
            _moviesCollection.InsertOne(movie);
        }
    }
}