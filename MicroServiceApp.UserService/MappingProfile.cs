using AutoMapper;
using MicroServiceApp.UserService.Dto;
using MicroServiceApp.UserService.Models;

namespace MicroServiceApp.UserService;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserDto, UserRegisteredMessage>()
            .ReverseMap();
    }
}