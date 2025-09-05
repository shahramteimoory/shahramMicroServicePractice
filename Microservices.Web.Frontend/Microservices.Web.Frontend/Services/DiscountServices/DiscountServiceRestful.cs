using Microservices.Web.Frontend.Models.Dtos;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace Microservices.Web.Frontend.Services.DiscountServices
{
    public class DiscountServiceRestful : IDiscountService
    {
        private readonly RestClient restClient;

        public DiscountServiceRestful(RestClient restClient)
        {
            this.restClient = restClient;
        }

        public ResultDto<DiscountDto> GetDiscountByCode(string Code)
        {
            var request = new RestRequest($"/api/Discount?code={Code}", Method.GET);
            IRestResponse response = restClient.Execute(request);
            return JsonConvert.DeserializeObject<ResultDto<DiscountDto>>(response.Content);
        }

        public ResultDto<DiscountDto> GetDiscountById(Guid Id)
        {
            var request = new RestRequest($"/api/Discount/{Id}", Method.GET);
            IRestResponse response = restClient.Execute(request);
            return JsonConvert.DeserializeObject<ResultDto<DiscountDto>>(response.Content);
        }

        public ResultDto UseDiscount(Guid DiscountId)
        {
            var request = new RestRequest("/api/Discount", Method.PUT);
            IRestResponse response = restClient.Execute(request);
            return JsonConvert.DeserializeObject<ResultDto>(response.Content);
        }
    }
}
