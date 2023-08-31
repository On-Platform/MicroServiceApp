using AutoMapper;
using MicroServiceApp.UserService.Dto;
using MicroServiceApp.UserService.Models;
using MicroServiceApp.UserService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MicroServiceApp.UserService.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UserService(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }
    public Task RegisterUserAsync(UserDto message)
    {
        try
        {
            var user = _mapper.Map<User>(message);
            _userRepository.AddUserAsync(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return Task.CompletedTask;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<List<UserDto>> GetUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetUsersAsync();
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return usersDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<UserDto> UpdateUserAsync(Guid id, UserUpdateDto userUpdateDto)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user = _mapper.Map(userUpdateDto, user);
            user = await _userRepository.UpdateUserAsync(user);
            var userDtoResponse = _mapper.Map<UserDto>(user);
            return userDtoResponse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}