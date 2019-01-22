using System;

namespace PRAPI.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Body { get; set; }
        public int Doors { get; set; }
        public string Fuel { get; set; }
        public string Transmission { get; set; }
        public string Description { get; set; }
        public float LP100Km { get; set; }
        public string AirConditioned { get; set; }
        public int Price { get; set; }
        public string PhotoUrl { get; set; }
        public string PublicId { get; set; }
    }
}