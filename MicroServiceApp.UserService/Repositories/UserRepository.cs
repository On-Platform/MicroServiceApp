using MicroServiceApp.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroServiceApp.UserService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        user = (await _dbContext.Users.AddAsync(user)).Entity;
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }
    
    public async Task<User> UpdateUserAsync(User user)
    {
        user = _dbContext.Users.Update(user).Entity;
        await _dbContext.SaveChangesAsync();
        return user;
    }
}