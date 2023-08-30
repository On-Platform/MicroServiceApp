using AutoMapper;
using SampleApp.UserService.Dto;
using SampleApp.UserService.Models;

namespace SampleApp.UserService;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}