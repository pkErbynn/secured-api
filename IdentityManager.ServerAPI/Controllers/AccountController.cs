using Microsoft.AspNetCore.Mvc;
using ShareClassLibrary.Contracts;
using ShareClassLibrary.Dtos;

namespace IdentityManager.ServerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        public readonly IUserAccount userAccount;

        public AccountController(IUserAccount userAccount)
        {
            this.userAccount = userAccount;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var userCreatedResponse = await userAccount.CreateAccountAsync(userDto);
            return Ok(userCreatedResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var userLoginResponse = await userAccount.LoginAccountAsync(loginDto);
            return Ok(userLoginResponse);
        }
        
    }
}