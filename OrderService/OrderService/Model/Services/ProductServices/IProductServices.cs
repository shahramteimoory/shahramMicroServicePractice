using OrderService.Infrastructure.Context;
using OrderService.Model.Entities;
using System;
using System.Linq;

namespace OrderService.Model.Services.ProductServices
{
    public interface IProductServices
    {
        Product GetProduct(PrroductDto prroductDto);
        bool UpdateProductName(Guid productId,string NewName);
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

        public bool UpdateProductName(Guid productId, string NewName)
        {
            var product = context.Products.FirstOrDefault(x=>x.ProductId==productId);
            if (product is null) return false;

            product.Name = NewName;
            context.SaveChanges();
            return true;
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
