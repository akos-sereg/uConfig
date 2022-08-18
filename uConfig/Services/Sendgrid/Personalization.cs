using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Services.Sendgrid
{
    public class Personalization
    {
        public List<EmailAddress> to { get; set; }

        public String subject { get; set; }

        public Personalization()
        {
        }

        public Personalization(EmailAddress to, String subject)
        {
            this.to = new List<EmailAddress>();
            this.to.Add(to);
            this.subject = subject;
        }
    }
}
