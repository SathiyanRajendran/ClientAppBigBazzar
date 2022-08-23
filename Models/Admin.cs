using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Special Characters not allowed")]
        [Display(Name = "Admin Name")]

        public string? AdminName { get; set; }
        [Required(ErrorMessage = "EmailId is Required")]
        [DataType(DataType.EmailAddress)]
        public string? AdminEmail { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "At least one uppercase, one lowercase, one digit, one special character and minimum eight in length")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [Display(Name = "Password")]

        public string? AdminPassword { get; set; }
        [DataType(DataType.Password)]

        [Compare("AdminPassword", ErrorMessage = "Password and Confirmation Password do not match.")]
        [NotMapped]
        [Display(Name = "Confirm Password")]

        public string? ConfirmPassword { get; set; }
    }
}
