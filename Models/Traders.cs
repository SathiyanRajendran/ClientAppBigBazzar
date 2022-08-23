using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class Traders
    {
        [Key]
        public int TraderId { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Special Characters not allowed")]

        public string? TraderName { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]

        [StringLength(10, MinimumLength = 10)]
        public string? TraderPhoneNumber { get; set; }
        [Required(ErrorMessage = "EmailId is Required")]
        [DataType(DataType.EmailAddress)]

        public string? TraderEmail { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "At least one uppercase, one lowercase, one digit, one special character and minimum eight in length")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]


        public string? Password { get; set; }
        [DataType(DataType.Password)]

        [Compare("Password", ErrorMessage = "Password and Confirmation Password do not match.")]
        [NotMapped]
        public string? ConfirmPassword { get; set; }
        public virtual ICollection<Products>? Products { get; set; }
    }
}
