using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Services.Sendgrid
{
    public class Content
    {
        public String type { get; set; }

        public String value { get; set; }

        public Content()
        {
        }

        public Content(String mimeType, String content)
        {
            this.type = mimeType;
            this.value = content;
        }
    }
}
