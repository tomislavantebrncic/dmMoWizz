using dmMoWizz.Models;
using dmMoWizz.Models.Mongo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace dmMoWizz.Repositories
{
    public class LocationRepository
    {
        private IMongoCollection<Location> locationCollection;

        public LocationRepository()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("dm-mowizz");
            locationCollection = database.GetCollection<Location>("locations");
        }

        public Location getLocation(string ip)
        {
            FilterDefinition<Location> locationFilter = Builders<Location>.Filter.Eq("IPAddress", ip);
            if (locationCollection.Find(locationFilter).CountDocuments() == 1)
            {
                return locationCollection.Find(locationFilter).First();
            }
            else
            {
                return null;
            }
        }

        public void saveLocation(Location location)
        {
            FilterDefinition<Location> locationFilter = Builders<Location>.Filter.Eq("IPAddress", location.IPAddress);
            if (locationCollection.Find(locationFilter).CountDocuments() == 0)
            {
                locationCollection.InsertOne(location);
            }
        }

        public void updateLocation(Location location)
        {
            FilterDefinition<Location> locationFilter = Builders<Location>.Filter.Eq("IPAddress", location.IPAddress);
            if (locationCollection.Find(locationFilter).CountDocuments() == 1)
            {
                locationCollection.ReplaceOne(locationFilter, location);
            }
            else if (locationCollection.Find(locationFilter).CountDocuments() == 0)
            {
                locationCollection.InsertOne(location);
            }
        }
    }
}