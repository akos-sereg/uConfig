using Microsoft.AspNetCore.Mvc;
using uConfig.DTOs;
using uConfig.Model;
using uConfig.Services;

namespace uConfig.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private AuthenticationService _authenticationService;
        public LoginController()
        {
            _authenticationService = new AuthenticationService();
        }

        [HttpPost]
        public LoggedInUser Login(LoginRequest loginRequest)
        {
            return _authenticationService.Login();
        }
    }
}
