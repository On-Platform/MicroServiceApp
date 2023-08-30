using AutoMapper;
using MicroServiceApp.UserService.Dto;
using MicroServiceApp.UserService.Models;

namespace MicroServiceApp.UserService.Services;

public class UserService : IUserService
{
    private readonly UserDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserService(UserDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task RegisterUserAsync(UserDto message)
    {
        using (_dbContext)
        {
            try
            {
                var user = _mapper.Map<User>(message);
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        return Task.CompletedTask;
    }

    public Task<UserDto> GetUserById(Guid id)
    {
        using (_dbContext)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                var userDto = _mapper.Map<UserDto>(user);
                return Task.FromResult(userDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}