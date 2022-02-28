﻿using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System;
using System.Threading.Tasks;
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
    }
}
