using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using uConfig.Services.Sendgrid;

namespace uConfig.Services
{
    public class EmailService
    {
        public static string FROM_EMAIL = "noreply@microconfy.com";
        public static string SENDGRID_APIKEY = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SendGrid")["ApiKey"];
        public static string SENDGRID_ENDPOINT_URL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SendGrid")["EndpointUrl"];

        public EmailService()
        {

        }

        public async Task SendRegistrationCode(string email, int registrationCode)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SENDGRID_APIKEY);

            MailSendRequest request = MailSendRequest.build(
                FROM_EMAIL,
                email,
                "Microconfy Registration Code",
                String.Format("Your registration code is: {0}", registrationCode),
                "text/plain");

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(SENDGRID_ENDPOINT_URL, data);
            Console.WriteLine("Sending JSON:");
            Console.WriteLine(json);
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Console.WriteLine("-------------------------------------------");
        }
    }
}
