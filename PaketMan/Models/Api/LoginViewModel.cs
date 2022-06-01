using System.ComponentModel.DataAnnotations;

namespace PaketMan.Models.Api
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
