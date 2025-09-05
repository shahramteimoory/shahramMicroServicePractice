using ApiGateWayForWeb.Models.DiscountServices;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateWayForWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController(IDiscountService discountService) : ControllerBase
    {
        [HttpGet]
        public ActionResult Get(string code)
        {
            return Ok(discountService.GetDiscountByCode(code));
        }

        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            return Ok(discountService.GetDiscountById(id));
        }

        [HttpPut]
        public ActionResult Put(Guid id)
        {
            return Ok(discountService.UseDiscount(id));
        }


    }
}
