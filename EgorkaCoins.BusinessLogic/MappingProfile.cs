using AutoMapper;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Пользователь -> DTO
            CreateMap<User, UserDto>();

            // Остальные сущности маппятся сами
            CreateMap<Game, Game>();
            CreateMap<Package, Package>();
            CreateMap<Order, Order>();
        }
    }
}
