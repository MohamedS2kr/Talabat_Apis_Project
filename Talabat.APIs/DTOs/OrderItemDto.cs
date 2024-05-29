using Talabat.Core.Entities.Order;

namespace Talabat.APIs.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public decimal price { get; set; }
        public int Quantity { get; set; }
    }
}