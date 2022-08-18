using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
    public class JwtTokenService
    {
		public static string JWT_SECRET = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt")["Key"];

		public static string GenerateToken(int userId, string email, string apiKey)
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

		public static LoggedInUser AuthenticateWithJwt(string token)
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
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
