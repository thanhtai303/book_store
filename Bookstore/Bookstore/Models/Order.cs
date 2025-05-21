namespace BookStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}