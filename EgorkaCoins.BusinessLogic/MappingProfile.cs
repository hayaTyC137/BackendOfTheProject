using AutoMapper;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User → UserDto
            CreateMap<User, UserDto>();

            // Game, Package, Order — маппятся сами (имена полей совпадают)
            CreateMap<Game, Game>();
            CreateMap<Package, Package>();
            CreateMap<Order, Order>();
        }
    }
}