using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Application.DTOs
{
    public class LocationDto
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
