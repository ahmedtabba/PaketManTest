using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PaketMan.Models.Api.Identity
{
    public class MyPasswordReseViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The Password cannot be empty.")]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        public string Password
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [DisplayName("Password confirmation")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [Compare("Password", ErrorMessage = "The entered passwords do not match.")]
        public string PasswordConfirmation
        {
            get;
            set;
        }
    }
}
