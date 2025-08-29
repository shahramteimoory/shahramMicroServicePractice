using AutoMapper;
using BasketService.Model.Entities;
using BasketService.Model.Services;
using BasketService.Model.Services.BasketServices;
using BasketService.Model.Services.BasketServices.MessagesDto;

namespace BasketService.Infrastructure.MappingProfile
{
    public class BasketMappingProfile : Profile
    {
        public BasketMappingProfile()
        {
            CreateMap<BasketItem, AddItemToBasketDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<AddItemToBasketDto, ProductDto>().ReverseMap();
            CreateMap<CheckoutBasketDto, BasketCheckOutMessage>().ReverseMap();

        }
    }
}
