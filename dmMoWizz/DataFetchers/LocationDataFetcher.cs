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
    public class LocationDataFetcher
    {
        private static IPStackResponse fetchData(string ip)
        {
            string ipStackAppID = ConfigurationManager.AppSettings["IPStackAppID"];
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "http";
            uriBuilder.Host = "api.ipstack.com";
            uriBuilder.Path = ip;
            uriBuilder.Query = "access_key=" + ipStackAppID;
            Uri ipStackRequestUri = uriBuilder.Uri;
            WebRequest webRequest = WebRequest.Create(ipStackRequestUri.ToString());
            WebResponse response = webRequest.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            try
            {
                IPStackResponse ipStackResponse = JsonConvert.DeserializeObject<IPStackResponse>(responseFromServer);
                return ipStackResponse;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private static Location convert(IPStackResponse ipStackResponse)
        {
            Location location = new Location()
            {
                IPAddress = ipStackResponse.ip,
                CityName = ipStackResponse.city,
                LastUpdate = System.DateTime.UtcNow.ToString()
            };

            return location;
        }

        public static Location getLocation(string ip)
        {
            int updateInterval = 60;

            LocationRepository locationCollectionController = new LocationRepository();

            Location location = locationCollectionController.getLocation(ip);
            if (location != null)
            {
                DateTime timeNow = System.DateTime.UtcNow;
                DateTime timeLastUpdate = DateTimeOffset.Parse(location.LastUpdate).UtcDateTime;

                if ((timeNow - timeLastUpdate).Minutes > updateInterval)
                {
                    IPStackResponse ipStackResponse = LocationDataFetcher.fetchData(ip);
                    Location newLocation = LocationDataFetcher.convert(ipStackResponse);

                    location.LastUpdate = newLocation.LastUpdate;
                    location.CityName = newLocation.CityName;
                    locationCollectionController.updateLocation(location);
                }
            }
            else
            {
                IPStackResponse ipStackResponse = LocationDataFetcher.fetchData(ip);
                location = LocationDataFetcher.convert(ipStackResponse);
                locationCollectionController.saveLocation(location);
            }

            return location;
        }
    }
}