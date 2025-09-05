using BasketService.Infrastructure.Contexts;
using System;
using System.Linq;

namespace BasketService.Model.Services.ProductServices
{
    public interface IProductService
    {
        bool UpdateProductName(Guid productId,string newName);
    }

    public class ProductSerrvice : IProductService
    {
        private readonly BasketDataBaseContext context;

        public ProductSerrvice(BasketDataBaseContext context)
        {
            this.context = context;
        }

        public bool UpdateProductName(Guid productId, string newName)
        {
            var product = context.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product is null) return false;

            product.ProductName = newName;
            context.SaveChanges();
            return true;

        }
    }
}
