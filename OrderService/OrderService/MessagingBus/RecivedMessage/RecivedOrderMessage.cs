using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService.Constants;
using OrderService.Infrastructure.Context;
using OrderService.Model.Services.MessagesDto;
using OrderService.Model.Services.ProductServices;
using OrderService.Model.Services.RegisterOrderServices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.MessagingBus.RecivedMessage
{
    public class RecivedOrderMessage : BackgroundService
    {
        private IConnection connection;
        private IModel model;
        IRegisterOrderService registerOrderService;
        private readonly OrderDataBaseContext context;
        private readonly IProductServices productServices;

        public RecivedOrderMessage(IOptions<RabbitMQConfig> options, IRegisterOrderService registerOrderService,OrderDataBaseContext context,IProductServices productServices)
        {
            this.productServices = productServices;
            this.registerOrderService = registerOrderService;
            var factory = new ConnectionFactory
            {
                HostName=options.Value.HostName,
                UserName=options.Value.UserName,
                Password=options.Value.Password
            };
            this.context = context;
            connection = factory.CreateConnection();
            model = connection.CreateModel();

            model.ExchangeDeclare(QueNames.UpdateProductNameExchange, ExchangeType.Fanout, true, false);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            BasketCheckOut();
            PaymentDoneStatus();
            ReciveChangeProductName();
            return Task.CompletedTask;
        }

        private void ReciveChangeProductName()
        {
            model.QueueDeclare(QueNames.UpdateProductNameQue, true, false, false);
            model.QueueBind(QueNames.UpdateProductNameQue, QueNames.UpdateProductNameExchange, "");

            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var updateCustomerFullNameModel = JsonConvert.DeserializeObject<UpdateProductNameMessage>(content);

                if (HandleChangeProductNameMessage(updateCustomerFullNameModel))
                    model.BasicAck(ea.DeliveryTag, false);
            };

            model.BasicConsume(QueNames.UpdateProductNameQue, false, consumer);
        }

        private void BasketCheckOut()
        {
            model.QueueDeclare(QueNames.BasketCheckOut,true,false,false,null);
            var counsumer = new EventingBasicConsumer(model);
            counsumer.Received += (sender, eventArgs) =>
            {
                var body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var result = JsonConvert.DeserializeObject<BasketCheckOutMessage>(body);

                if(HandleBasketCheckoutMessage(result))
                    model.BasicAck(eventArgs.DeliveryTag, false);
            };

            model.BasicConsume(QueNames.BasketCheckOut,false,consumer: counsumer);
        }

        private void PaymentDoneStatus()
        {
            model.QueueDeclare(QueNames.PaymentDone, true, false, false, null);
            var counsumer = new EventingBasicConsumer(model);

            counsumer.Received += (sender, eventArgs) =>
            {
                var body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var result = JsonConvert.DeserializeObject<PaymentDoneDto>(body);

                if (HandlePaymentDoneMessage(result))
                    model.BasicAck(eventArgs.DeliveryTag, false);
            };

            model.BasicConsume(QueNames.PaymentDone, false, consumer: counsumer);
        }

        private bool HandleChangeProductNameMessage(UpdateProductNameMessage request)
        {
            return productServices.UpdateProductName(request.Id, request.NewName);
        }

        private bool HandlePaymentDoneMessage(PaymentDoneDto orderid)
        {
            var order = context.Orders.FirstOrDefault(x => x.Id == orderid.Orderid);
            if (order is null) return false;

            order.PaymentIsDone();
            context.SaveChanges();
            return true;
        }

        private bool HandleBasketCheckoutMessage(BasketCheckOutMessage request)
        {
           return registerOrderService.Execute(request);
        }
    }

    public class PaymentDoneDto
    {
        public Guid Orderid { get; set; }
    }

    public class UpdateProductNameMessage
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
    }
}
