using System;
using System.Collections.Generic;
using System.Linq;
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
                ApiKey = "7612354561234781256347123564"
            };
        }
    }
}
