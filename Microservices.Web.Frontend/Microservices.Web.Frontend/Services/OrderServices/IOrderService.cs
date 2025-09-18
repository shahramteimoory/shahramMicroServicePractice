using Duende.IdentityModel.Client;
using Microservices.Web.Frontend.Models.Dtos;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microservices.Web.Frontend.Services.OrderServices
{
    public interface IOrderService
    {
        List<OrderDto> GetOrders(string UserId);
        OrderDetailDto OrderDetail(Guid OrderId);
        ResultDto RequestPayment(Guid OrderId);

    }

    public class OrderService : IOrderService
    {
        private readonly RestClient restClient;

        private  string _accessToken = null;
        public OrderService(RestClient restClient)
        {
            this.restClient = restClient;
            restClient.Timeout = -1;
        }

        private async Task<string> GetAccessToken()
        {
            if (!string.IsNullOrWhiteSpace(_accessToken))
            {
                return _accessToken;
            }
            HttpClient httpClient = new HttpClient();
            var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:7036");
            var token = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address= discovery.TokenEndpoint,
                ClientId= "webfrontend",
                ClientSecret="123456",
                Scope= "orderservice.Fullaccess"
            });
            if (token.IsError) throw new Exception(token.Error);

           return _accessToken = token.AccessToken;
        }

        private void CreateRequestheader(RestRequest request)
        {
            var token = GetAccessToken().GetAwaiter().GetResult();
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        public List<OrderDto> GetOrders(string UserId)
        {
            var request = new RestRequest("/api/Order", Method.GET);
            CreateRequestheader(request);
            IRestResponse response = restClient.Execute(request);
            var orders = JsonConvert.DeserializeObject<List<OrderDto>>(response.Content);
            return orders;
        }

        public OrderDetailDto OrderDetail(Guid OrderId)
        {
            var request = new RestRequest($"/api/Order/{OrderId}", Method.GET);
            CreateRequestheader(request);
            IRestResponse response = restClient.Execute(request);
            var orderdetail = JsonConvert.DeserializeObject<OrderDetailDto>(response.Content);
            return orderdetail;
        }

        public ResultDto RequestPayment(Guid OrderId)
        {
            var request = new RestRequest($"/api/OrderPayment?OrderId={OrderId}", Method.POST);
            CreateRequestheader(request);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = restClient.Execute(request);
            return GetResponseStatusCode(response);
        }
        private static ResultDto GetResponseStatusCode(IRestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new ResultDto
                {
                    IsSuccess = true,
                };
            }
            else
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = response.ErrorMessage
                };
            }
        }
    }
}
