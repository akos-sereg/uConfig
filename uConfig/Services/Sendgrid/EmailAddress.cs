using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace uConfig.Services.Sendgrid
{
    public class EmailAddress
    {
        public String email { get; set; }

        public EmailAddress(String email)
        {
            this.email = email;
        }
    }
}
