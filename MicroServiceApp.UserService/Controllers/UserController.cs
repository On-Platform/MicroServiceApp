using System.Security.Claims;
using MicroServiceApp.UserService.Dto;
using MicroServiceApp.UserService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroServiceApp.UserService.Controllers;

[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    
    public UserController(IUserService userService, IConfiguration configuration)
    {
        _configuration = configuration;
        _userService = userService;
    }
    
    [HttpGet(template: "api/users/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        return Ok(await _userService.GetUserByIdAsync(id));
    }
    
    [HttpGet(template: "api/users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userService.GetUsersAsync());
    }
    
    //Update User
    [HttpPut(template: "api/users/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto userDto)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(await _userService.UpdateUserAsync(id, userDto));
    }
}