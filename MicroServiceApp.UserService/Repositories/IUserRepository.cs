using MicroServiceApp.UserService.Models;

namespace MicroServiceApp.UserService.Repositories;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<List<User>> GetUsersAsync();
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
}