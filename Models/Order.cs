namespace PRAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public string IsPaid { get; set; }
        public Car OrderedCar { get; set; }
        public User UserOrdering { get; set; }
    }
}