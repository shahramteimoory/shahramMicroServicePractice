using BasketService.Model.Services.BasketServices.MessagesDto;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BasketService.MessagingBus
{
    public class RabbitMQMessageBus : RabbitMQBus<BasketCheckOutMessage>
    {
        public RabbitMQMessageBus(IOptions<RabbitMQConfig> options):base(options)
        {}

        public override void SendMessage(BasketCheckOutMessage message, string queName)
        {
            if (CheckRabbitMQConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queName, durable:true,exclusive:false,autoDelete:false,
                        arguments:null);
                    var jsonMessage = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(jsonMessage);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange:"",routingKey: queName,properties,body);
                    return;
                }
            }
            CheckRabbitMQConnection();
            SendMessage(message,queName);
        }
    }
}
