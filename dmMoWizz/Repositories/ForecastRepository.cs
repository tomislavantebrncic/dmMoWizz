﻿using dmMoWizz.Models;
using dmMoWizz.Models.Mongo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace dmMoWizz.Repositories
{
    public class ForecastsRepository
    {
        private IMongoCollection<Forecast> forecastsCollection;

        public ForecastsRepository()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("dm-mowizz");
            forecastsCollection = database.GetCollection<Forecast>("forecasts");
        }

        public Forecast getForecast(string cityName)
        {
            FilterDefinition<Forecast> forecastsFilter = Builders<Forecast>.Filter.Eq("CityName", cityName);
            if (forecastsCollection.Find(forecastsFilter).CountDocuments() == 1)
            {
                return forecastsCollection.Find(forecastsFilter).First();
            }
            else
            {
                return null;
            }
        }

        public void saveForecast(Forecast forecast)
        {
            FilterDefinition<Forecast> forecastsFilter = Builders<Forecast>.Filter.Eq("CityName", forecast.CityName);
            if (forecastsCollection.Find(forecastsFilter).CountDocuments() == 0)
            {
                forecastsCollection.InsertOne(forecast);
            }
        }

        public void updateForecast(Forecast forecast)
        {
            FilterDefinition<Forecast> forecastsFilter = Builders<Forecast>.Filter.Eq("CityName", forecast.CityName);
            if (forecastsCollection.Find(forecastsFilter).CountDocuments() == 1)
            {
                forecastsCollection.ReplaceOne(forecastsFilter, forecast);
            }
            else if (forecastsCollection.Find(forecastsFilter).CountDocuments() == 0)
            {
                forecastsCollection.InsertOne(forecast);
            }
        }
    }
}