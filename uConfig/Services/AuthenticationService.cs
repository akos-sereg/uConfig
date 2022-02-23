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
        public LoggedInUser GetLoggedInUser()
        {
            return new LoggedInUser() { 
                Email = "akos.sereg@gmail.com", 
                UserID = 1234,
                ApiKey = "7612354561234781256347123564",
				Token = this.GenerateToken(1234)
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
