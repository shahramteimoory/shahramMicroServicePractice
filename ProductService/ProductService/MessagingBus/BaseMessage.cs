using ProductService.Constants;
using System;

namespace ProductService.MessagingBus
{
    public class BaseMessage
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }

    public class UpdateProductMessage: BaseMessage
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }

    }
}
