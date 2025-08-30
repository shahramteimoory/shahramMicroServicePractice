using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace OrderService.MessagingBus
{
    public abstract class RabbitMQBus<T>
    {
        protected RabbitMQConfig rabbitMQConfig { get; private set; }
        protected IConnection _connection;
        public RabbitMQBus(IOptions<RabbitMQConfig> options)
        {
            rabbitMQConfig = options.Value;
        }
        public abstract void SendMessage(T message, string queName);

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
                _connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                //log
                throw;
            }
        }

        public virtual bool CheckRabbitMQConnection()
        {
            if (_connection != null)
                return true;

            CreateRabbitMQConnections();
            return _connection != null;

        }
    }
}
