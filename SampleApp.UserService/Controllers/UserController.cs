using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleApp.UserService.Services;

namespace SampleApp.UserService.Controllers;

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
    public IActionResult GetUserById(Guid id)
    {
        return Ok(_userService.GetUserById(id));
    }
    
    [HttpGet(template: "api/users")]
    public IActionResult RegisterUser()
    {
        return Ok(_configuration["IdentityServer:Authority"]);
    }
}