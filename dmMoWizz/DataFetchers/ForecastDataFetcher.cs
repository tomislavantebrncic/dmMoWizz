using System;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.IO;
using System.Collections.Generic;

using dmMoWizz.Models;
using dmMoWizz.Models.Mongo;
using dmMoWizz.Repositories;

namespace dmMoWizz.DataFetchers
{
    public class ForecastDataFetcher
    {
        private static OpenWeatherMapResponse fetchData(string city)
        {
            string openWeatherMapAppID = ConfigurationManager.AppSettings["OpenWeatherMapAppID"];
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "api.openweathermap.org";
            uriBuilder.Path = "data/2.5/forecast";
            uriBuilder.Query = "q=" + city + "&APPID=" + openWeatherMapAppID;
            Uri openWeatherMapRequestUri = uriBuilder.Uri;
            WebRequest webRequest = WebRequest.Create(openWeatherMapRequestUri.ToString());
            WebResponse response = webRequest.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            OpenWeatherMapResponse openWeatherMapResponse = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(responseFromServer);

            return openWeatherMapResponse;
        }

        private static Forecast convert(OpenWeatherMapResponse openWeatherMapResponse)
        {
            Forecast forecast = new Forecast()
            {
                CityName = openWeatherMapResponse.city.name,
                LastUpdate = System.DateTime.UtcNow.ToString(),
                Data = new List<ForecastDataModel>()
            };
            foreach (ForecastData data in openWeatherMapResponse.list)
            {
                forecast.Data.Add(new ForecastDataModel() {
                    Time = data.dt_txt,
                    Description = data.weather[0].description,
                    Main = data.weather[0].main,
                    Temperature = data.main.temp,
                    WindSpeed = data.wind.speed,
                    Icon = "http://openweathermap.org/img/w/" + data.weather[0].icon + ".png"
                });
            }

            return forecast;
        }

        public static Forecast getForecast(string city)
        {
            int updateInterval = int.Parse(ConfigurationManager.AppSettings["ForecastUpdateInterval"]);

            ForecastsRepository forecastsCollectionController = new ForecastsRepository();
            Forecast forecast = forecastsCollectionController.getForecast(city);
            if (forecast != null)
            {
                DateTime timeNow = System.DateTime.UtcNow;
                DateTime timeLastUpdate = DateTimeOffset.Parse(forecast.LastUpdate).UtcDateTime;

                if ((timeNow - timeLastUpdate).Minutes > updateInterval)
                {
                    OpenWeatherMapResponse openWeatherMapResponse = ForecastDataFetcher.fetchData(city);
                    Forecast newForecast = ForecastDataFetcher.convert(openWeatherMapResponse);
                    forecast.LastUpdate = newForecast.LastUpdate;
                    forecast.Data.Clear();
                    foreach(ForecastDataModel data in newForecast.Data)
                    {
                        forecast.Data.Add(data);
                    }
                    forecastsCollectionController.updateForecast(forecast);
                }
            }
            else
            {
                OpenWeatherMapResponse openWeatherMapResponse = ForecastDataFetcher.fetchData(city);
                forecast = ForecastDataFetcher.convert(openWeatherMapResponse);
                forecastsCollectionController.saveForecast(forecast);
            }

            return forecast;
        }
    }
}