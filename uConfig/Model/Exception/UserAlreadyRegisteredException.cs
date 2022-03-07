using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model.Exception
{
    public class UserAlreadyRegisteredException : System.Exception
    {
        public string Email { get; set; }

        public UserAlreadyRegisteredException(string email, string message) : base(message)
        {
            this.Email = email;
        }
    }
}
