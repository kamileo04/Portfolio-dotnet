using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email")]
        public string Email { get; set; }

        public string? Subject { get; set; }

        [Required(ErrorMessage = "Wiadomość jest wymagana")]
        public string Message { get; set; }
    }
}