using Microsoft.Extensions.Caching.Distributed;
using RedisSample.Models.Entities;
using System.Text.Json;

namespace RedisSample.Models.Services
{
    public interface ICartServices
    {
        Task<Cart> GetCart(string userName);
        Task<Cart> UpdateCart(Cart cart);
        Task RemoveCart(string userName);
    }
    public class CartServices(IDistributedCache cache) : ICartServices
    {

        public async Task<Cart> GetCart(string userName)
        {
            var cart =await cache.GetStringAsync(userName);

            if (cart is null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<Cart>(cart);
        }

        public async Task RemoveCart(string userName)
        {
            await cache.RemoveAsync(userName);
        }

        public async Task<Cart> UpdateCart(Cart cart)
        {
            await cache.SetStringAsync(cart.UserName, JsonSerializer.Serialize(cart));
            return await GetCart(cart.UserName);
        }
    }
}
