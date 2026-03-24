using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        // Wstrzykujemy konfiguracjê, by pobraæ dane SMTP z appsettings.json
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
                try
                {
                    var host = _configuration["BrevoSmtp:Host"];
                    var port = int.Parse(_configuration["BrevoSmtp:Port"]);
                    var username = _configuration["BrevoSmtp:Username"];
                    var password = _configuration["BrevoSmtp:Password"];

                    using (var client = new SmtpClient(host, port))
                    {
                        client.Credentials = new NetworkCredential(username, password);
                        client.EnableSsl = true;

                        var mailMessage = new MailMessage
                        {
                            // U¿ywamy z autoryzowanego konta e-mail do wysy³ki jako nadawca
                            From = new MailAddress("kamileo04@gmail.com", "Portfolio Formularz"),
                            Subject = $"Nowa wiadomoœæ z portfolio: {model.Subject ?? "Brak tematu"}",
                            Body = $"Wiadomoœæ od: {model.Name} ({model.Email})\n\nTreœæ:\n{model.Message}",
                            IsBodyHtml = false,
                        };

                        // Aby móc normalnie "Odpowiedzieæ" na maila klikaj¹c w przycisk Odpowiedz:
                        mailMessage.ReplyToList.Add(new MailAddress(model.Email, model.Name));

                        // Tutaj wpisz SWÓJ adres email, na który chcesz otrzymywaæ wiadomoœci
                        mailMessage.To.Add("kamileo04@gmail.com");

                        client.Send(mailMessage);
                    }

                    ViewBag.Message = "Wiadomoœæ zosta³a wys³ana pomyœlnie!";
                    ModelState.Clear(); // Czyszczenie formularza
                    return View(new ContactViewModel());
                }
                catch (Exception ex)
                {
                    // Wyrzucenie pe³nego b³êdu w oknie konsoli Output (Debug)
                    System.Diagnostics.Debug.WriteLine("\n=== B£¥D WYSY£ANIA MAILA ===");
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    System.Diagnostics.Debug.WriteLine("============================\n");

                    // Rzutowanie pe³nego b³êdu do logu w zwyk³ej konsoli ASP.NET:
                    Console.WriteLine("\n=== B£¥D WYSY£ANIA MAILA ===");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("============================\n");

                    // Rozbudowany log b³êdu na stronie
                    ViewBag.Error = $"Wyst¹pi³ b³¹d podczas wysy³ania wiadomoœci (szczegó³y w konsoli). Komunikat: {ex.Message}";
                }
            }
            else
            {
                // Diagnostyka b³êdów walidacji formularza, jeœli mail w ogóle nie przeszed³ walidacji
                System.Diagnostics.Debug.WriteLine("\n=== B£ÊDY WALIDACJI FORMULARZA ===");
                foreach(var modelState in ModelState.Values)
                {
                    foreach(var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"B³¹d pol: {error.ErrorMessage}");
                    }
                }
                System.Diagnostics.Debug.WriteLine("==================================\n");
            }

            // Jeœli formularz jest niepoprawny, wracamy do widoku wyœwietlaj¹c b³êdy
            return View(model);
        }
    }
}
