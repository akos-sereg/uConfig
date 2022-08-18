using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Services.Sendgrid
{
    public class MailSendRequest
    {
        
        public List<Personalization> personalizations { get; set; }
        public EmailAddress from { get; set; }
        public List<Content> content { get; set; }

        public MailSendRequest()
        {
        }

        public static MailSendRequest build(String from, String to, String subject, String content, String mimeType)
        {
            MailSendRequest request = new MailSendRequest();
            List<Personalization> p = new List<Personalization>();
            p.Add(new Personalization(new EmailAddress(to), subject));
            request.personalizations = p;
            request.from = new EmailAddress(from);
            request.content = new List<Content>();
            request.content.Add(new Content(mimeType, content));

            return request;
        }
    }
}
