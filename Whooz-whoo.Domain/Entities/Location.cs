using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Entities
{
    public class Location : ValueObject
    {
        public string Address { get; private set; }
        public string City { get; private set; }
        public string Province { get; private set; }
        public string Country { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Location(string address, string city, string province, string country,
                        double latitude, double longitude)
        {
            Address = address;
            City = city;
            Province = province;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address;
            yield return City;
            yield return Province;
            yield return Country;
            yield return Latitude;
            yield return Longitude;
        }

        public double CalculateDistance(Location other)
        {
            // Haversine formula implementation
            const double R = 6371; // Earth's radius in kilometers
            var lat1 = Latitude * Math.PI / 180;
            var lat2 = other.Latitude * Math.PI / 180;
            var diffLat = (other.Latitude - Latitude) * Math.PI / 180;
            var diffLon = (other.Longitude - Longitude) * Math.PI / 180;

            var a = Math.Sin(diffLat / 2) * Math.Sin(diffLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(diffLon / 2) * Math.Sin(diffLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
