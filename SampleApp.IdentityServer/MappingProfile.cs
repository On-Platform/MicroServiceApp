using AutoMapper;
using SampleApp.IdentityServer.Dto;
using SampleApp.IdentityServer.Models;

namespace SampleApp.IdentityServer;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegisteredMessage, UserDto>().ReverseMap();
        CreateMap<UserDto, ApplicationUser>().ReverseMap();
    }
}