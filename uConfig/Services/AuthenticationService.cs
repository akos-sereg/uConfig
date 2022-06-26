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

namespace uConfig.Services
{
    public class AuthenticationService
    {
		private string connectionString;
		private string JWT_SECRET = null;
		private string demoUser = null;
		private string demoJwtToken = null;

		public AuthenticationService(string connectionString)
        {
			this.JWT_SECRET = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt")["Key"];
			this.demoUser = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Demo")["Username"];
			this.demoJwtToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Demo")["JwtToken"];
			this.connectionString = connectionString;
		}

		public async Task<LoggedInUser> Authenticate(string authorization)
		{
			if (string.IsNullOrEmpty(authorization))
            {
				return null;
            }

			if (authorization.StartsWith("apikey"))
            {
				return await AuthenticateWithApiKey(authorization.Replace("apikey", "").Trim());
            } else
            {
				return AuthenticateWithJwt(authorization);
            }
		}

		public async Task<LoggedInUser> Login(string username, string password)
        {
			// log in with demo user and empty password: login endpoint will return a valid JWT token for the demo user,
			// but with read-only access (eg. role is "demo")
			if (!string.IsNullOrEmpty(username) && username.Equals(this.demoUser) && string.IsNullOrEmpty(password))
            {
				return await GetDemoUser();
			}

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();
				using var command = new MySqlCommand("SELECT user_id, email, password, api_key, registered_at FROM uconfy_user WHERE email = ?username AND password = MD5(?password)", connection);
				command.Parameters.Add("?username", MySqlDbType.VarChar).Value = username;
				command.Parameters.Add("?password", MySqlDbType.VarChar).Value = password;

				using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					LoggedInUser user = new LoggedInUser()
					{
						ApiKey = reader.GetString(reader.GetOrdinal("api_key")),
						Email = reader.GetString(reader.GetOrdinal("email")),
						UserID = reader.GetInt32(reader.GetOrdinal("user_id"))
					};

					user.Token = GenerateToken(user.UserID, user.Email, user.ApiKey);

					return user;
				}

				return null;
			}
		}

		private async Task<LoggedInUser> GetDemoUser()
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();
				using var command = new MySqlCommand("SELECT user_id, email, password, api_key, registered_at FROM uconfy_user WHERE email = ?username", connection);
				command.Parameters.Add("?username", MySqlDbType.VarChar).Value = this.demoUser;

				using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					LoggedInUser user = new LoggedInUser()
					{
						ApiKey = reader.GetString(reader.GetOrdinal("api_key")),
						Email = reader.GetString(reader.GetOrdinal("email")),
						UserID = reader.GetInt32(reader.GetOrdinal("user_id"))
					};

					user.Token = this.demoJwtToken;
					return user;
				}

				return null;
			}
		}

		private async Task<LoggedInUser> AuthenticateWithApiKey(string apiKey)
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();
				using var command = new MySqlCommand("SELECT user_id, email, password, api_key, registered_at FROM uconfy_user WHERE api_key = ?api_key", connection);
				command.Parameters.Add("?api_key", MySqlDbType.VarChar).Value = apiKey;

				using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					LoggedInUser user = new LoggedInUser()
					{
						ApiKey = reader.GetString(reader.GetOrdinal("api_key")),
						Email = reader.GetString(reader.GetOrdinal("email")),
						UserID = reader.GetInt32(reader.GetOrdinal("user_id")),
					};

					user.Role = user.Email.Equals("demouser@demo.de") ? "demo" : "user";
					user.Token = GenerateToken(user.UserID, user.Email, user.ApiKey);

					return user;
				}

				return null;
			}
		}

		public LoggedInUser AuthenticateWithJwt(string token)
        {
			try
            {
				var key = Encoding.ASCII.GetBytes(JWT_SECRET);
				var handler = new JwtSecurityTokenHandler();
				var validations = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false
				};

				var claims = handler.ValidateToken(token, validations, out var tokenSecure);
				return new LoggedInUser()
				{
					Email = claims.Claims.First(claim => claim.Type == ClaimTypes.Email).Value,
					UserID = int.Parse(claims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value),
					ApiKey = claims.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value,
					Role = claims.Claims.First(claim => claim.Type == ClaimTypes.Role).Value,
					Token = token
				};
			} catch (Exception)
            {
				return null;
            }	
		}

		public async Task<Boolean> IsEmailRegistered(string email) {
			int userCount = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();

				// check for existing user
				using var command = new MySqlCommand("SELECT user_id FROM uconfy_user WHERE email = ?email", connection);
				command.Parameters.Add("?email", MySqlDbType.VarChar).Value = email.Trim().ToLower();

				using var reader = await command.ExecuteReaderAsync();
				
				while (await reader.ReadAsync())
				{
					userCount++;
				}

				await connection.CloseAsync();
			}

			return userCount > 0;
		}

		public async Task RegisterUser(string email, string password)
        {
			bool isUserRegistered = await IsEmailRegistered(email);
			if (isUserRegistered)
            {
				throw new UserAlreadyRegisteredException(email, "Unable to register user");
			}

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();

				// insert
				var insertCommand = new MySqlCommand("INSERT INTO uconfy_user (user_id, email, password, api_key, registered_at) VALUES (DEFAULT, ?email, MD5(?password), ?api_key, UTC_TIMESTAMP())", connection);
				insertCommand.Parameters.Add("?email", MySqlDbType.VarChar).Value = email.Trim().ToLower();
				insertCommand.Parameters.Add("?password", MySqlDbType.VarChar).Value = password;
				insertCommand.Parameters.Add("?api_key", MySqlDbType.VarChar).Value = Guid.NewGuid().ToString();
				await insertCommand.ExecuteNonQueryAsync();
			}
		}

		private string GenerateToken(int userId, string email, string apiKey)
		{
			var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWT_SECRET));
			var myIssuer = "http://mysite.com";
			var myAudience = "http://myaudience.com";
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
					new Claim(ClaimTypes.Email, email),
					new Claim(ClaimTypes.Sid, apiKey),
					new Claim(ClaimTypes.Role, "user") // role should be "demo" when generating read-only user (eg. demo user's jwt token for public use)
				}),
				Expires = DateTime.UtcNow.AddDays(365),
				Issuer = myIssuer,
				Audience = myAudience,
				SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
