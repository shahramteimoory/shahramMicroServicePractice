using OrderService.Infrastructure.Context;
using OrderService.Model.Entities;
using OrderService.Model.Services.MessagesDto;
using OrderService.Model.Services.ProductServices;
using System;
using System.Collections.Generic;

namespace OrderService.Model.Services.RegisterOrderServices
{
    public interface IRegisterOrderService
    {
        bool Execute(BasketCheckOutMessage basketCheckOutMessage);
    }

    public class RegisterOrderService : IRegisterOrderService
    {
        private readonly IProductServices productServices;
        private readonly OrderDataBaseContext context;

        public RegisterOrderService(IProductServices productServices, OrderDataBaseContext context)
        {
            this.productServices = productServices;
            this.context = context;
        }

        public bool Execute(BasketCheckOutMessage basketCheckOutMessage)
        {
            List<OrderLine> orderLineDtos = new List<OrderLine>();
            foreach (var item in basketCheckOutMessage.basketItemMessages)
            {
                var product = productServices.GetProduct(new PrroductDto
                {
                    Name=item.Name,
                    Price=item.Price,
                    ProductId=item.ProductId
                });

                orderLineDtos.Add(new OrderLine
                {
                    Id=Guid.NewGuid(),
                    Quantity=item.Quantity,
                    Product=product
                });
            }

            Order order = new Order(basketCheckOutMessage.UserId, basketCheckOutMessage.FirstName, basketCheckOutMessage.LastName,
                basketCheckOutMessage.Address, basketCheckOutMessage.PhoneNumber, basketCheckOutMessage.TotalPrice, orderLineDtos);

            context.Orders.Add(order);
            context.SaveChanges();
            return true;
        }
    }
}
