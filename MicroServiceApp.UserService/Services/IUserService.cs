using MicroServiceApp.UserService.Dto;

namespace MicroServiceApp.UserService.Services;

public interface IUserService
{
    Task RegisterUserAsync(UserDto message);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto> UpdateUserAsync(Guid id, UserUpdateDto userUpdateDto);
}