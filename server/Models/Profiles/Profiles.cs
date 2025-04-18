using AutoMapper;
using server.Models.DTO;

namespace server.Models.Profiles
{
    public class DrinkProfile : Profile
    {
        public DrinkProfile()
        {
            CreateMap<Drink, DrinkDto>()
                .ForMember(dest => dest.IsAvailable,
                    opt => opt.MapFrom(src => src.Amount > 0))
                .ForMember(dest => dest.Brand,
                    opt => opt.MapFrom(src => src.Brand));

            CreateMap<Brand, BrandDto>();

            CreateMap<DrinkCreateDto, Drink>();
            CreateMap<DrinkUpdateDto, Drink>();
        }
    }

    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderItems,
                    opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDto>();
        }
    }

    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, BrandDto>();
            CreateMap<BrandCreateDto, Brand>();
            CreateMap<BrandUpdateDto, Brand>();
        }
    }
}