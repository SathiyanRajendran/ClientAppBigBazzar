using System.ComponentModel.DataAnnotations;

namespace ClientAppBigBazzar.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
