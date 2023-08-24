using AutoMapper;
using ProxyFill.Model;
using ProxyFill.Shared.ViewModel;

namespace ProxyFill.Mapper;

public class ProxyCardProfile : Profile
{
    public ProxyCardProfile()
    {
        CreateMap<ProxyCardViewModel, ProxyCardDTO>()
            .ForMember(dest => dest.FrontImage, opt => opt.MapFrom(src => src.FrontImage.Download))
            .ForMember(dest => dest.BackImage, opt => opt.MapFrom(src => src.BackImage.Download));

    }
}