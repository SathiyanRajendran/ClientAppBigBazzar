using System.ComponentModel.DataAnnotations;

namespace ClientAppBigBazzar.Models
{
    public class Login
    {
        [Key]
        [Required(ErrorMessage = "CustomerName is Required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}
