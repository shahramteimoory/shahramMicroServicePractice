using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Constants;
using ProductService.MessagingBus;
using ProductService.Model.Services;
using System;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductManagmentController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IMessageBus messageBus;
        public ProductManagmentController(IProductService productService, IMessageBus messageBus)
        {
            this.productService = productService;
            this.messageBus = messageBus;
        }

        [HttpPost]
        public ActionResult Post([FromBody] AddNewProductDto request)
        {
            productService.AddNewProduct(request);
            return Ok();
        }

        [HttpGet]
        public ActionResult Get()
        {
            var products = productService.GetProductList();
            return Ok(products);
        }

        [HttpGet("id")]
        public ActionResult Get(Guid id)
        {
            var products = productService.GetProduct(id);
            return Ok(products);
        }

        [HttpPut]
        public ActionResult Put([FromBody] UpdateProductDto request)
        {
            var products = productService.UpdateProduct(request);
            if (products)
            {
                var message = new UpdateProductMessage()
                {
                    NewName=request.Name,
                    Id=request.productId
                };
                messageBus.SendMessage(message,QueNames.UpdateProductNameExchange);
            }

            return Ok(products);
        }

    }
}
