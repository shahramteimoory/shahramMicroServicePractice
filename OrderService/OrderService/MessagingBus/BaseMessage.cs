using System;

namespace OrderService.MessagingBus
{
    public class BaseMessage
    {
        public Guid MeesageId { get; set; } = Guid.NewGuid();
        public DateTime CreateTime { get; set; } = DateTime.Now;

    }
}
