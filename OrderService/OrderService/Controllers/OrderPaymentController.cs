using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Model.Services;
using System;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPaymentController : ControllerBase
    {
        private readonly IOrderService orderService;
        public OrderPaymentController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost]
        public ActionResult Post(Guid orderId)
        {
            return Ok(orderService.RequestPayment(orderId));
        }
    }
}
