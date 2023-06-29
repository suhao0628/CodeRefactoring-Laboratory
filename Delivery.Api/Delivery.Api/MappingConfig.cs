using AutoMapper;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Model;

namespace Delivery.Api
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Dish, DishDto>().ReverseMap();
            
            CreateMap<OrderCreateDto, Order>();

            CreateMap<Order, OrderInfoDto>();

            CreateMap<User, UserDto>();
        }
    }
}
