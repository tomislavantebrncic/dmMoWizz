using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace dmMoWizz.Models.Mongo
{
    public class Location
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("LastUpdate")]
        public string LastUpdate { get; set; }

        [BsonElement("IPAddress")]
        public string IPAddress { get; set; }

        [BsonElement("CityName")]
        public string CityName { get; set; }
    }
}