using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Entities;

namespace Whooz_whoo.Domain.Interfaces
{
    public interface IGeolocationService
    {
        Task<Location> GetLocationByAddressAsync(string address);
        Task<Location> GetCurrentLocationAsync();
        Task<double> CalculateDistanceAsync(Location location1, Location location2);
        Task<bool> ValidateLocationAsync(Location location);
        Task<string> GetAddressFromCoordinatesAsync(double latitude, double longitude);
    }
}
