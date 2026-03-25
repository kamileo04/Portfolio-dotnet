using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username = null;
                string password = null;
                try
                {
                    var host = _configuration["BrevoSmtp:Host"];
                    var port = int.Parse(_configuration["BrevoSmtp:Port"]);
                    username = _configuration["BrevoSmtp:Username"];
                    password = _configuration["BrevoSmtp:Password"];

                    using (var client = new SmtpClient(host, port))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(username, password);
                        client.EnableSsl = true;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("kamileo04@gmail.com", "Portfolio Formularz"),
                            Subject = $"Nowa wiadomość z portfolio: {model.Subject ?? "Brak tematu"}",
                            Body = $"Wiadomość od: {model.Name} ({model.Email})\n\nTreść:\n{model.Message}",
                            IsBodyHtml = false,
                        };

                        mailMessage.ReplyToList.Add(new MailAddress(model.Email, model.Name));

                        mailMessage.To.Add("kamileo04@gmail.com");

                        client.Send(mailMessage);
                    }

                    ViewBag.Message = "Wiadomość została wysłana pomyślnie!";
                    ModelState.Clear();
                    return View(new ContactViewModel());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n=== BŁĄD WYSYŁANIA MAILA ===");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("============================\n");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("\n=== BŁĘDY WALIDACJI FORMULARZA ===");
                foreach(var modelState in ModelState.Values)
                {
                    foreach(var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd pol: {error.ErrorMessage}");
                    }
                }
                System.Diagnostics.Debug.WriteLine("==================================\n");
            }

            return View(model);
        }
    }
}
