using System;

namespace OrderService.Model.Services
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public int ItemCount { get; set; }
        public int TotalPrice { get; set; }
        public bool OrderPaid { get; set; }
        public DateTime OrderPlaced { get; set; }

    }
}
