using MicroServiceApp.UserService.Dto;

namespace MicroServiceApp.UserService.Services;

public interface IUserService
{
    Task RegisterUserAsync(UserDto message);
    Task<UserDto> GetUserById(Guid id);
}