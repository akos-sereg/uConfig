using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.Services
{
    public class AuthenticationService
    {
		private string connectionString;
		private static string JWT_SECRET = "egy vegkep betort allat hatan, egy vegkep betort allat lovagol";

		public AuthenticationService(string connectionString)
        {
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
						UserID = reader.GetInt32(reader.GetOrdinal("user_id"))
					};

					user.Token = GenerateToken(user.UserID, user.Email, user.ApiKey);

					return user;
				}

				return null;
			}
		}

		private LoggedInUser AuthenticateWithJwt(string token)
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
					Token = token
				};
			} catch (Exception)
            {
				return null;
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
					new Claim(ClaimTypes.Sid, apiKey)
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
