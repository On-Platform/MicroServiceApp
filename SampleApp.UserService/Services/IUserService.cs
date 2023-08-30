using SampleApp.UserService.Dto;

namespace SampleApp.UserService.Services;

public interface IUserService
{
    Task RegisterUserAsync(UserDto message);
    Task<UserDto> GetUserById(Guid id);
}