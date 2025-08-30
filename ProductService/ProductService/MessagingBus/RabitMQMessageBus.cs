using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductService.MessagingBus.Config;
using RabbitMQ.Client;
using System;
using System.Text;

namespace ProductService.MessagingBus
{
    public class RabitMQMessageBus: IMessageBus
    {
        private readonly RabbitMQConfig rabbitMQConfig;
        private IConnection connection;
        public RabitMQMessageBus(IOptions<RabbitMQConfig> options)
        {
            rabbitMQConfig = options.Value;
            CreateRabbitMQConnections();
        }

        public void SendMessage(BaseMessage baseMessage, string exchange)
        {
            if (CheckRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(exchange,ExchangeType.Fanout,true,false,null);
                    var jsonMessage = JsonConvert.SerializeObject(baseMessage);
                    var body = Encoding.UTF8.GetBytes(jsonMessage);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: exchange, routingKey:"", properties, body);
                    return;
                }
            }
            CheckRabbitMQConnection();
            SendMessage(baseMessage, exchange);
        }

        public virtual void CreateRabbitMQConnections()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = rabbitMQConfig.HostName,
                    UserName = rabbitMQConfig.UserName,
                    Password = rabbitMQConfig.Password,
                };
                connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                //log
                throw;
            }
        }

        public virtual bool CheckRabbitMQConnection()
        {
            if (connection != null)
                return true;

            CreateRabbitMQConnections();
            return connection != null;

        }

    }
}
