using AutoMapper;
using MicroServiceApp.IdentityServer.Dto;
using MicroServiceApp.IdentityServer.Models;

namespace MicroServiceApp.IdentityServer;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegisteredMessage, UserDto>().ReverseMap();
        CreateMap<UserDto, ApplicationUser>()
            .ForMember(d=>d.UserName, opt=>opt.MapFrom(s=>s.Email))
            .ReverseMap();
    }
}