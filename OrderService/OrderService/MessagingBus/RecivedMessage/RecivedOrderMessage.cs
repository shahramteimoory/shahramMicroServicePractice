using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService.Constants;
using OrderService.Model.Services.MessagesDto;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.MessagingBus.RecivedMessage
{
    public class RecivedOrderMessage : BackgroundService
    {
        private IConnection connection;
        private IModel model;
        private readonly RabbitMQConfig rabbitMQConfig;

        public RecivedOrderMessage(IOptions<RabbitMQConfig> options)
        {
            rabbitMQConfig = options.Value;

            var factory = new ConnectionFactory
            {
                HostName=options.Value.HostName,
                UserName=options.Value.UserName,
                Password=options.Value.Password
            };

            connection = factory.CreateConnection();
            model = connection.CreateModel();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            BasketCheckOut();

            return Task.CompletedTask;
        }

        private void BasketCheckOut()
        {
            model.QueueDeclare(QueNames.BasketCheckOut,true,false,false,null);
            var counsumer = new EventingBasicConsumer(model);
            counsumer.Received += (sender, eventArgs) =>
            {
                var body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var result = JsonConvert.DeserializeObject<BasketCheckOutMessage>(body);

                if(HandleMessage(result))
                    model.BasicAck(eventArgs.DeliveryTag, false);
            };

            model.BasicConsume(QueNames.BasketCheckOut,false,consumer: counsumer);
        }

        private bool HandleMessage<T>(T request)
        {
            return false;
        }
    }
}
