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
                return Ok();
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

        [HttpPost]
        [Route("verify-email")]
        public async Task<IActionResult> CompleteSignup(CompleteSignupRequest completeSignupRequest)
        {
            Console.WriteLine("Complete Signup: '{0}', code: {1}", completeSignupRequest.Email, completeSignupRequest.RegistrationCode);
            try
            {
                LoggedInUser user = await _authenticationService.CompleteSignup(completeSignupRequest.Email, completeSignupRequest.RegistrationCode);
                if (user != null)
                {
                    return Ok(user);
                } else
                {
                    return Unauthorized();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return StatusCode(500);
            }
        }
    }
}
