using System;

namespace PRAPI.Models
{
    public class SearchParams
    {
        public DateTime ReservedFrom { get; set; }
        public DateTime ReservedTo { get; set; }
        public String Model { get; set; }
    }
}