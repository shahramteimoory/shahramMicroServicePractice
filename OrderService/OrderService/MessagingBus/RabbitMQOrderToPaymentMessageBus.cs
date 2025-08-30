using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.MessagingBus
{
    public class RabbitMQOrderToPaymentMessageBus : RabbitMQBus<SendOrderToPaymentMessage>
    {
        public RabbitMQOrderToPaymentMessageBus(IOptions<RabbitMQConfig> options) : base(options)
        { }

        public override void SendMessage(SendOrderToPaymentMessage message, string queName)
        {
            if (CheckRabbitMQConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queName, durable: true, exclusive: false, autoDelete: false,
                        arguments: null);
                    var jsonMessage = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(jsonMessage);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "", routingKey: queName, properties, body);
                    return;
                }
            }
            CheckRabbitMQConnection();
            SendMessage(message, queName);
        }
    }
}
