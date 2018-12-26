using System;
using PRAPI.Models;

namespace PRAPI.Dtos
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public string IsReturned { get; set; }
        public DateTime? ReservedFrom { get; set; }
        public DateTime? ReservedTo { get; set; }
        public Car CarOrdered { get; set; }
    }
}