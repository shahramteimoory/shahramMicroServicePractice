using System;
using System.Collections.Generic;

namespace OrderService.Model.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; private set; }
        public DateTime OrderPlaced { get; private set; }
        public bool OrderPaid { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public int TotalPrice { get; set; }
        public OrderStatus PaymentStatus { get; private set; }

        public ICollection<OrderLine> OrderLines { get; private set; }

        public Order(string UserId,
            string FirstName,
            string LastName,
            string Address,
            string PhoneNumber,
            int TotalPrice,
            List<OrderLine> OrderLines)
        {
            this.UserId = UserId;
            this.OrderPaid = false;
            this.OrderPlaced = DateTime.Now;
            this.OrderLines = OrderLines;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
            this.TotalPrice = TotalPrice;
            this.PaymentStatus = OrderStatus.unPaid;
        }
        public Order() { }

        public void RequestPayment()
        {
            PaymentStatus = OrderStatus.RequestPayment;
        }

        public void PaymentIsDone()
        {
            OrderPaid = true;
            PaymentStatus = OrderStatus.isPaid;
        }
    }

    public enum OrderStatus
    {
        unPaid,
        RequestPayment,
        isPaid,

    }


}
