using BasketService.Constants;
using BasketService.Model.Services.ProductServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasketService.MessagingBus.RecivedMessages.ProductMessages
{
    public class RecivedUpdateProductName : BackgroundService
    {
        private IModel model;
        private IConnection connection;
        private readonly RabbitMQConfig rabbitMQConfig;
        private readonly IProductService productService;
        public RecivedUpdateProductName(IOptions<RabbitMQConfig> options,IProductService productService)
        {
            this.productService = productService;
            rabbitMQConfig = options.Value;
            var factory=new ConnectionFactory
            {
                HostName=options.Value.HostName,
                UserName=options.Value.UserName,
                Password=options.Value.Password
            };
            connection = factory.CreateConnection();
            model = connection.CreateModel();

            model.ExchangeDeclare(QueNames.UpdateProductNameExchange,ExchangeType.Fanout,true,false);
            model.QueueDeclare(QueNames.UpdateProductNameQue,true,false,false);
            model.QueueBind(QueNames.UpdateProductNameQue,QueNames.UpdateProductNameExchange,"");
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var updateCustomerFullNameModel = JsonConvert.DeserializeObject<UpdateProductNameMessage>(content);

                if (HandleMessage(updateCustomerFullNameModel))
                    model.BasicAck(ea.DeliveryTag,false);
            };

            model.BasicConsume(QueNames.UpdateProductNameQue,false,consumer);
            return Task.CompletedTask;
        }


        private bool HandleMessage(UpdateProductNameMessage message)
        {
            return productService.UpdateProductName(message.Id,message.NewName);
        }
    }
    public class UpdateProductNameMessage
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
    }
}
