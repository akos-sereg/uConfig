using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System;
using System.Threading.Tasks;
using uConfig.DTOs;
using uConfig.Model;
using uConfig.Model.Exception;
using uConfig.Services;

namespace uConfig.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private AuthenticationService _authenticationService;
        public LoginController(MySqlConnection connection)
        {
            _authenticationService = new AuthenticationService(connection.ConnectionString);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _authenticationService.Login(loginRequest.Username, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("jwt")]
        public IActionResult Validate(ValidateJwtRequest validateJwt)
        {
            Console.WriteLine("Auth JWT: '{0}'", validateJwt.JwtToken);
            var user = _authenticationService.AuthenticateWithJwt(validateJwt.JwtToken);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup(SignupRequest signupRequest)
        {
            Console.WriteLine("Signup: '{0}'", signupRequest.Email);
            try
            {
                if (string.IsNullOrEmpty(signupRequest.Password) 
                    || signupRequest.Password.Length < 6 
                    || string.IsNullOrEmpty(signupRequest.Email))
                {
                    return BadRequest();
                }

                await _authenticationService.RegisterUser(signupRequest.Email, signupRequest.Password);
                var user = await _authenticationService.Login(signupRequest.Email, signupRequest.Password);
                if (user == null)
                {
                    return Unauthorized();
                }

                return Ok(user);
            } catch (UserAlreadyRegisteredException error)
            {
                Console.WriteLine(error.Message);
                return Conflict();
            } catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return StatusCode(500);
            }
        }
    }
}
