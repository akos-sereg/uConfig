using Microsoft.AspNetCore.Mvc;
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
    public class AuthenticationService
    {
        public LoggedInUser Login()
        {
            return new LoggedInUser() { 
                Email = "akos.sereg@gmail.com", 
                UserID = 1234,
                ApiKey = "7612354561234781256347123564",
				Token = this.GenerateToken(1234)
            };
        }

		public LoggedInUser GetLoggedInUser(string token)
		{
			Console.WriteLine("GetLoggedInUser: {0}", token);

			/*var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenS = jsonToken as JwtSecurityToken;
			
			var email = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
			int userId = int.Parse(tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
			var apiKey = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value;
			return new LoggedInUser()
			{
				Email = email,
				UserID = userId,
				ApiKey = apiKey,
				Token = token
			};*/

			string secret = "asdv2sdafsdf34234^&%&^%&^hjsdfb2%%%";
			var key = Encoding.ASCII.GetBytes(secret);
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

		}

		public string GenerateToken(int userId)
		{
			var mySecret = "asdv2sdafsdf34234^&%&^%&^hjsdfb2%%%";
			var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
			var myIssuer = "http://mysite.com";
			var myAudience = "http://myaudience.com";
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
					new Claim(ClaimTypes.Email, "akos.sereg@gmail.com"),
					new Claim(ClaimTypes.Sid, "7612354561234781256347123564")
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
