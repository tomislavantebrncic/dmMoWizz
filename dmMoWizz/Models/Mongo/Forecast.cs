using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace dmMoWizz.Models.Mongo
{
    public class ForecastDataModel
    {
        [BsonElement("Time")]
        public string Time { get; set; }
        [BsonElement("Temperature")]
        public string Temperature { get; set; }
        [BsonElement("Main")]
        public string Main { get; set; }
        [BsonElement("Description")]
        public string Description { get; set; }
        [BsonElement("WindSpeed")]
        public string WindSpeed { get; set; }
        [BsonElement("Icon")]
        public string Icon { get; set; }
    }
    public class Forecast
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("LastUpdate")]
        public string LastUpdate { get; set; }
        [BsonElement("CityName")]
        public string CityName { get; set; }
        [BsonElement("Data")]
        public IList<ForecastDataModel> Data { get; set; }
    }
}