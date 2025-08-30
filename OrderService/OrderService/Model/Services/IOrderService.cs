using Microsoft.EntityFrameworkCore;
using OrderService.Constants;
using OrderService.Infrastructure.Context;
using OrderService.MessagingBus;
using OrderService.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderService.Model.Services
{
    public interface IOrderService
    {

        List<OrderDto> GetOrdersForUser(string UserId);
        OrderDetailDto GetOrderById(Guid Id);
        ResultDto RequestPayment(Guid orderId);
    }


    public class OrderService : IOrderService
    {
        private readonly OrderDataBaseContext context;
        private readonly RabbitMQBus<SendOrderToPaymentMessage> rabbitMQBus;

        public OrderService(OrderDataBaseContext context, RabbitMQBus<SendOrderToPaymentMessage> rabbitMQBus)
        {
            this.context = context;
            this.rabbitMQBus = rabbitMQBus;
        }

        public OrderDetailDto GetOrderById(Guid Id)
        {
            var orders = context.Orders.
                   Include(p => p.OrderLines)
                  .ThenInclude(p => p.Product)
                 .FirstOrDefault(p => p.Id == Id);

            if (orders == null)
                throw new Exception("Order Not Found");
            var result = new OrderDetailDto
            {
                Id = orders.Id,
                Address = orders.Address,
                FirstName = orders.FirstName,
                LastName = orders.LastName,
                PhoneNumber = orders.PhoneNumber,
                UserId = orders.UserId,
                OrderPaid = orders.OrderPaid,
                OrderPlaced = orders.OrderPlaced,
                TotalPrice = orders.TotalPrice,
                OrderLines = orders.OrderLines.Select(ol => new OrderLineDto
                {
                    Id = ol.Id,
                    Name = ol.Product.Name,
                    Price = ol.Product.Price,
                    Quantity = ol.Quantity,

                }).ToList(),

            };
            return result;

        }

        public List<OrderDto> GetOrdersForUser(string UserId)
        {
            var orders = context.Orders.
             Include(p => p.OrderLines)
            .Where(p => p.UserId == UserId)
            .Select(p => new OrderDto
            {
                Id = p.Id,
                OrderPaid = p.OrderPaid,
                OrderPlaced = p.OrderPlaced,
                ItemCount = p.OrderLines.Count(),
                TotalPrice = p.TotalPrice,
            }).ToList();
            return orders;
        }

        public ResultDto RequestPayment(Guid orderId)
        {
            var order = context.Orders.FirstOrDefault(x => x.Id == orderId);
            if (order is null)
            {
                return new ResultDto
                {
                    Message = "موردی یافت نشد"
                };
            }
            var message = new SendOrderToPaymentMessage()
            {
                Amount=order.TotalPrice,
                OrderId=order.Id
            };

            rabbitMQBus.SendMessage(message, QueNames.OrderSendToPayment);

            order.RequestPayment();
            context.SaveChanges();

            return new ResultDto
            {
                IsSuccess = true,
                Message = "درخواست پرداخت ثبت شد"
            };
        }
    }
}
