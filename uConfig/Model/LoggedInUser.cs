using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class LoggedInUser
    {
        public string Email { get; set; }

        public int UserID { get; set; }

        /// <summary>
        /// OAuth token for current web session
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// API Key that can be used by devices to fetch configuration
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Either "demo" or "user". Demo user is read-only.
        /// </summary>
        public string Role { get; set; }

	}
}
