using OrderService.Infrastructure.Context;
using OrderService.Model.Entities;
using System;
using System.Linq;

namespace OrderService.Model.Services.ProductServices
{
    public interface IProductServices
    {
        Product GetProduct(PrroductDto prroductDto);
    }

    public class ProductService : IProductServices
    {
        private readonly OrderDataBaseContext context;
        public ProductService(OrderDataBaseContext context)
        {
            this.context = context;
        }
        public Product GetProduct(PrroductDto prroductDto)
        {
            var existProduct = context.Products.FirstOrDefault(x=>x.ProductId==prroductDto.ProductId);
            if (existProduct is null)
                return CreateNewProduct(prroductDto);

            return existProduct;
        }

        private Product CreateNewProduct(PrroductDto prroductDto)
        {
            Product product = new Product()
            {
                ProductId = prroductDto.ProductId,
                Name = prroductDto.Name,
                Price = prroductDto.Price,
            };

            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }
    }

    public class PrroductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
