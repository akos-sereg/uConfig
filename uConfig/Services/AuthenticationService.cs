using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using uConfig.Model;
using uConfig.Model.Exception;
using uConfig.Repository;

namespace uConfig.Services
{
    public class AuthenticationService
    {
		private EmailService emailService;
		private UserRepository userRepository;

		public AuthenticationService(string connectionString)
        {
			this.emailService = new EmailService();
			this.userRepository = new UserRepository(connectionString);
		}

		public async Task<LoggedInUser> Authenticate(string authorization)
		{
			if (string.IsNullOrEmpty(authorization))
            {
				return null;
            }

			if (authorization.StartsWith("apikey"))
            {
				return await this.userRepository.GetUserByApiKey(authorization.Replace("apikey", "").Trim());
            } else
            {
				return JwtTokenService.AuthenticateWithJwt(authorization);
            }
		}

		public LoggedInUser AuthenticateWithJwt(string token)
        {
			return JwtTokenService.AuthenticateWithJwt(token);
        }

		public async Task<LoggedInUser> Login(string username, string password)
        {
			// log in with demo user and empty password: login endpoint will return a valid JWT token for the demo user,
			// but with read-only access (eg. role is "demo")
			if (!string.IsNullOrEmpty(username) && username.Equals(this.userRepository.DemoUser) && string.IsNullOrEmpty(password))
            {
				return await this.userRepository.GetDemoUser();
			}

			return await this.userRepository.GetUserWithVerifiedEmail(username, password);
		}

		public async Task RegisterUser(string email, string password)
        {
			bool isUserRegistered = await this.userRepository.IsEmailRegistered(email);
			if (isUserRegistered)
            {
				LoggedInUser user = await this.userRepository.GetUserByEmail(email);
				if (!user.EmailVerified)
                {
					await this.userRepository.DeleteUserByEmail(email);
                } else
                {
					throw new UserAlreadyRegisteredException(email, user.EmailVerified, "Unable to register user");
				}
			}

			Random rnd = new Random();
			int registrationCode = rnd.Next(100000, 999999);
			await userRepository.RegisterUserWithEmailNotVerified(email, password, registrationCode);
			await emailService.SendRegistrationCode(email, registrationCode);
		}

		public async Task<LoggedInUser> CompleteSignup(string email, int registrationCode)
        {
			LoggedInUser user = await this.userRepository.GetUserByEmail(email);
			if (registrationCode == user.RegistrationCode)
            {
				await this.userRepository.EmailVerified(email);
				user.EmailVerified = true;
				return user;
            } else
            {
				return null;
            }
        }
	}
}
