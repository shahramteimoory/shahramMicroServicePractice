using ApiGateWayForWeb.Models.DiscountServices.Dto;
using DiscountService.Proto;
using Grpc.Net.Client;
using System.Net;
using System.Threading.Channels;

namespace ApiGateWayForWeb.Models.DiscountServices
{
    public interface IDiscountService
    {
        ApiResult<DiscountDto> GetDiscountByCode(string Code);
        ApiResult<DiscountDto> GetDiscountById(Guid Id);
        ApiResult UseDiscount(Guid DiscountId);
    }
    public class DiscountServiceClass(GrpcChannel channel) : IDiscountService
    {
        public ApiResult<DiscountDto> GetDiscountByCode(string Code)
        {
            var grpc_discountService = new
                DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.GetDiscountByCode(new RequestGetDiscountByCode
            {
                Code = Code,
            });


            if (result.IsSuccess)
            {
                return new ApiResult<DiscountDto>
                {
                    Data = new DiscountDto
                    {
                        Amount = result.Data.Amount,
                        Code = result.Data.Code,
                        Id = Guid.Parse(result.Data.Id),
                        Used = result.Data.Used
                    },
                    IsSuccess = result.IsSuccess,
                    Message = result.Message,
                    StatusCode=HttpStatusCode.OK
                };
            }
            return new ApiResult<DiscountDto>
            {
                IsSuccess = false,
                Message = result.Message,
                StatusCode=HttpStatusCode.InternalServerError
            };
        }

        public ApiResult<DiscountDto> GetDiscountById(Guid Id)
        {
            var grpc_discountService = new DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.GetDiscountById(new RequestGetDiscountById
            {
                Id = Id.ToString(),
            });

            if (result.IsSuccess)
            {
                return new ApiResult<DiscountDto>
                {
                    Data = new DiscountDto
                    {
                        Amount = result.Data.Amount,
                        Code = result.Data.Code,
                        Id = Guid.Parse(result.Data.Id),
                        Used = result.Data.Used
                    },
                    IsSuccess = result.IsSuccess,
                    Message = result.Message,
                    StatusCode=HttpStatusCode.OK
                };
            }
            return new ApiResult<DiscountDto>
            {
                IsSuccess = false,
                Message = result.Message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        public ApiResult UseDiscount(Guid DiscountId)
        {
            var grpc_discountService = new DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.UseDiscount(new RequestUseDiscount
            {
                Id = DiscountId.ToString(),
            });
            return new ApiResult
            {
                IsSuccess = result.IsSuccess,
                StatusCode=HttpStatusCode.OK
            };
        }
    }
}
