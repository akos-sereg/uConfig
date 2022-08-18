using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class CompleteSignupRequest
    {
        public string Email { get; set; }

        public int RegistrationCode { get; set; }
    }
}
