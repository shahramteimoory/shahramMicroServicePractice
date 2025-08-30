using System;

namespace OrderService.MessagingBus
{
    public class BaseMessage
    {
        public Guid MeesageId { get; set; } = Guid.NewGuid();
        public DateTime CreateTime { get; set; } = DateTime.Now;

    }

    public class SendOrderToPaymentMessage: BaseMessage
    {
        public Guid OrderId { get; set; }
        public int Amount { get; set; }
    }
}
