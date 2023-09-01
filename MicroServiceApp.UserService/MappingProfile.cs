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
        CreateMap<User, UserDeletionMessage>();
        
        CreateMap<User, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        
        //Create a Map for UserUpdateDto and User ignoring null values
        CreateMap<UserUpdateDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}