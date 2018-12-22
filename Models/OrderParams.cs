using System;

namespace PRAPI.Models
{
    public class OrderParams
    {
        public DateTime ReservedFrom { get; set; }
        public DateTime ReservedTo { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
    }
}