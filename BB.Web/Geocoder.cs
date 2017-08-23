using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BB.Web
{
    public class Geocoder
    {
        private string _apiKey;

        private static WebClient _client = new WebClient();

        public Geocoder(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Place LookupByAddress(string address)
        {
            string encodedAddress = HttpUtility.UrlEncode(address);
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={_apiKey}";
            string json = _client.DownloadString(url);
            return Place.ParseJson(json);
        }

        public Place LookupByLatLng(double latitude, double longitude)
        {
            string formattedLat = latitude.ToString("0.000000");
            string formattedLng = longitude.ToString("0.000000");
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={formattedLat},{formattedLng}&key={_apiKey}";
            string json = _client.DownloadString(url);
            return Place.ParseJson(json);
        }
    }
}
