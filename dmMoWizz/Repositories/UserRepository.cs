using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dmMoWizz.Models;
using dmMoWizz.Models.Mongo;
using MongoDB.Driver;

namespace dmMoWizz.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<UserInfo> _usersCollection;

        public UserRepository()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = client.GetDatabase("dm-mowizz");

            _usersCollection = db.GetCollection<UserInfo>("users");
        }

        public void Add(ApplicationUser user)
        {
            var userInfo = new UserInfo
            {
                Id = user.Id,
                Ratings = new HashSet<MovieRating>(),
                Watchlist = new HashSet<WatchlistMovie>(),
                Watched = new HashSet<int>()
            };

            _usersCollection.InsertOne(userInfo);
        }

        public UserInfo Get(string id)
        {
            return _usersCollection.Find(u => u.Id.Equals(id)).FirstOrDefault();
        }

        public async void Update(UserInfo user)
        {
            var filter = Builders<UserInfo>.Filter.Eq(u => u.Id, user.Id);

            await _usersCollection.ReplaceOneAsync(filter, user);
        }
    }
}