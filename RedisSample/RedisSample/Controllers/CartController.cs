using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisSample.Models.Entities;
using RedisSample.Models.Services;
using System.Net;
using System.Threading.Tasks;

namespace RedisSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartServices cartServices) : ControllerBase
    {
        [HttpGet("{userName}",Name ="GetCart")]
        [ProducesResponseType(typeof(Cart),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> GetCart(string userName)
        {
            return Ok(await cartServices.GetCart(userName) ?? new Cart(userName));
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveCart(string userName)
        {
            await cartServices.RemoveCart(userName);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> AddCart(Cart cart)
        {
            return Ok(await cartServices.UpdateCart(cart));
        }
    }
}
