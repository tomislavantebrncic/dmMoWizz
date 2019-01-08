using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models
{
    public class Coordinates
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }
	
    public class City
    {
        public string id { get; set; }
        public string name { get; set; }
        public Coordinates coord { get; set; }
        public string country { get; set; }
    }

    public class ForecastMain
    {
        public string temp { get; set; }
        public string temp_min { get; set; }
        public string temp_max { get; set; }
        public string pressure { get; set; }
        public string sea_level { get; set; }
        public string grnd_level { get; set; }
        public string humidity { get; set; }
        public string temp_kf { get; set; }
    }
    public class ForecastWeather
    {
        public string id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }
    public class ForecastClouds
    {
        public string all { get; set; }
    }
    public class ForecastWind
    {
        public string speed { get; set; }
        public string deg { get; set; }
    }
    public class ForecastSys
    {
        public string pod { get; set; }
    }
    public class ForecastData
    {
        public string dt { get; set; }
        public ForecastMain main { get; set; }
        public IList<ForecastWeather> weather { get; set; }
        public ForecastClouds clouds { get; set; }
        public ForecastWind wind { get; set; }
        public ForecastSys sys { get; set; }
        public string dt_txt { get; set; }
    }

    public class OpenWeatherMapResponse
    {
        public string cod { get; set; }
        public string message { get; set; }
        public string cnt { get; set; }
        public IList<ForecastData> list { get; set; }
        public City city { get; set; }
    }
}