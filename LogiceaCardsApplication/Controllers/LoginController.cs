using LogiceaCardsApplication.Services;
using LogiceaDTO;
using Microsoft.AspNetCore.Mvc;

namespace LogiceaCardsApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly LookUpService _lookUpService;
        private readonly JwtService _jwtService;
        public LoginController(IConfiguration configuration, LookUpService lookUpService, JwtService jwtService) 
        {
            _configuration = configuration;
            _lookUpService = lookUpService;
            _jwtService = jwtService;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _lookUpService.AuthenticateUser(loginModel.UserId, loginModel.Password);
            // Perform basic validation
            if (user != null)
            {
                int expireAfterHours = Convert.ToInt32(_configuration.GetValue<string>("Jwt:ExpiresInHours"));
                var token = _jwtService.GenerateToken(user.Email, user.Email, user.Role, expireAfterHours);
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid credentials");
        }
    }
}
