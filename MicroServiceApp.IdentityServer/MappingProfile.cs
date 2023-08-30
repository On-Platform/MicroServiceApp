using AutoMapper;
using MicroServiceApp.IdentityServer.Dto;
using MicroServiceApp.IdentityServer.Models;

namespace MicroServiceApp.IdentityServer;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegisteredMessage, UserDto>().ReverseMap();
        CreateMap<UserDto, ApplicationUser>().ReverseMap();
    }
}