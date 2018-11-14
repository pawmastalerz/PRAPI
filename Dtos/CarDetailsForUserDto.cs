using System;

namespace PRAPI.Dtos
{
    public class CarDetailsForUserDto
    {
        public int Id { get; set; }
        public string License { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public int Doors { get; set; }
        public int Price { get; set; }
        public int Year { get; set; }
        public float LP100Km { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime ReservedFrom { get; set; }
        public DateTime ReservedTo { get; set; }
    }
}