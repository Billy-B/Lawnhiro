using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Web
{
    public sealed class Place
    {
        internal Place() { }

        private static System.Web.Script.Serialization.JavaScriptSerializer _serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

        class GooglePlace
        {
            public AddressComponent[] address_components;
            public string adr_address, formatted_address, name, place_id, url;
            public Geometry geometry;
        }

        class AddressComponent
        {
            public string long_name, short_name;
            public string[] types;
        }

        class Geometry
        {
            public Location location;
            public Viewport viewport;
        }

        class Location
        {
            public double lat, lng;
        }

        class Viewport
        {
            public double north, south, east, west;
        }

        private static string getAddressComponentShortName(IEnumerable<AddressComponent> components, string type)
        {
            AddressComponent component = components.FirstOrDefault(c => c.types.Contains(type));
            if (component == null)
            {
                return null;
            }
            return component.short_name;
        }

        private static string getAddressComponentLongName(IEnumerable<AddressComponent> components, string type)
        {
            AddressComponent component = components.FirstOrDefault(c => c.types.Contains(type));
            if (component == null)
            {
                return null;
            }
            return component.long_name;
        }

        internal static Place ParseJson(string json)
        {
            GooglePlace place = _serializer.Deserialize<GooglePlace>(json);
            AddressComponent[] components = place.address_components;
            return new Place
            {
                City = getAddressComponentLongName(components, "locality"),
                Country = getAddressComponentLongName(components, "country"),
                CountryAbbreviation = getAddressComponentShortName(components, "country"),
                County = getAddressComponentLongName(components, "administrative_area_level_2"),
                Latitude = place.geometry.location.lat,
                Longitude = place.geometry.location.lng,
                Name = place.name,
                Neighborhood = getAddressComponentLongName(components, "neighborhood"),
                PlaceId = place.place_id,
                PostalCode = getAddressComponentLongName(components, "postal_code"),
                PostalCodeSuffix = getAddressComponentLongName(components, "postal_code_suffix"),
                StateProvince = getAddressComponentLongName(components, "administrative_area_level_1"),
                StateProvinceAbbreviation = getAddressComponentShortName(components, "administrative_area_level_1"),
                StreetName = getAddressComponentLongName(components, "route"),
                StreetNumber = getAddressComponentLongName(components, "street_number")
            };
        }

        public string Name { get; private set; }

        public string PlaceId { get; private set; }

        public string City { get; private set; }

        public string StateProvince { get; private set; }

        public string StateProvinceAbbreviation { get; private set; }

        public string Country { get; private set; }

        public string CountryAbbreviation { get; private set; }

        public string PostalCode { get; private set; }

        public string PostalCodeSuffix { get; private set; }

        public string StreetName { get; private set; }

        public string StreetNumber { get; private set; }

        public string Neighborhood { get; private set; }

        public string County { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public override int GetHashCode()
        {
            return PlaceId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Place other = obj as Place;
            return other != null && other.PlaceId == PlaceId;
        }
    }
}
