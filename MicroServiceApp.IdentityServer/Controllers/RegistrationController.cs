using AutoMapper;
using MicroServiceApp.IdentityServer.Dto;
using MicroServiceApp.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MicroServiceApp.IdentityServer;

[ApiController]
public class RegistrationController : ControllerBase
{
    readonly IServiceBusSender<UserRegisteredMessage> _messageSender;
    readonly UserManager<ApplicationUser> _userManager;
    readonly IMapper _mapper;
    public RegistrationController(IServiceBusSender<UserRegisteredMessage> messageSender, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _mapper = mapper;
        _messageSender = messageSender;
        _userManager = userManager;
    }
    
    [HttpPost(template: "auth/register")]
    public async Task Add([FromBody] UserDto userDto)
    {
        var user = _mapper.Map<ApplicationUser>(userDto);
        user.Id = Guid.NewGuid();
        var identityUser = await _userManager.CreateAsync(user);
        if(identityUser.Succeeded)
        {
            var message = _mapper.Map<UserRegisteredMessage>(userDto);
            message.Id = user.Id;
            await _messageSender.SendAsync(message);
        }
        else
        {
            throw new Exception(identityUser.Errors.First().Description);
        }
    }
}