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
    private readonly IServiceBusSender<UserDeletionMessage> _serviceBusSender;

    public UserService(IMapper mapper, IUserRepository userRepository, IServiceBusSender<UserDeletionMessage> serviceBusSender)
    {
        _serviceBusSender = serviceBusSender;
        _mapper = mapper;
        _userRepository = userRepository;
    }
    public async Task RegisterUserAsync(UserDto message)
    {
        try
        {
            var user = _mapper.Map<User>(message);
            await _userRepository.AddUserAsync(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
    
    public async Task DeleteUserAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            await _userRepository.DeleteUserAsync(user);
            var userDeletionMessage = _mapper.Map<UserDeletionMessage>(user);
            await _serviceBusSender.SendAsync(userDeletionMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}