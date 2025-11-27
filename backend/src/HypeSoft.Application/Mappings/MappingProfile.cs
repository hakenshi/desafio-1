using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Entities;

namespace HypeSoft.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock()));
        
        CreateMap<Category, CategoryDto>();
    }
}
