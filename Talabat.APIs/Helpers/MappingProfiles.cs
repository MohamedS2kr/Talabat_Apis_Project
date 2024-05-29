using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember( DTos => DTos.Category , option => option.MapFrom(p => p.Category.Name))
                .ForMember( Dtos => Dtos.Brand , option => option.MapFrom(P => P.Brand.Name))
                .ForMember(Dtos => Dtos.PictureUrl , option => option.MapFrom<ProductPictureUrlResolver>());

            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<AddressDto, Core.Entities.Order.Address>();
            CreateMap<Order, OrderToReturnDTOs>()
                .ForMember(D=> D.DeliveryMethod , option => option.MapFrom(S=>S.DeliveryMethod.ShortName))
                .ForMember(D=> D.DeliveryMethodCost , option => option.MapFrom(S=>S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, Op => Op.MapFrom(S => S.product.ProductId))
                .ForMember(d => d.ProductId, Op => Op.MapFrom(S => S.product.ProductName))
                .ForMember(d => d.ProductId, Op => Op.MapFrom(S => S.product.PictureUrl));
            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();
        }

    }
}
