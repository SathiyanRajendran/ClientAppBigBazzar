using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class RegisterForm
    {
        [Key]
        public int CustomerId { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Special Characters not allowed")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "EmailId is Required")]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "At least one uppercase, one lowercase, one digit, one special character and minimum eight in length")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]

        public string Password { get; set; }
        [DataType(DataType.Password)]

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        [NotMapped]

        public string ConfirmPassword { get; set; }
        [Display(Name = "Mobile Number:")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]


        public string MobileNumber { get; set; }
        public string CustomerCity { get; set; }
        [DataType(DataType.PostalCode)]

        public string Pincode { get; set; }
        [Required]
        public string SecurityQuestion { get; set; }
        [Required]
        public string Answer { get; set; }
        public bool Status { get; set; }
        public string SecurityCode { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Captcha { get; set; }
        [Required(ErrorMessage = "Please enter captcha")]
        public string resultCaptcha { get; set; }
        //public virtual ICollection<Carts> Carts { get; set; }
        //public virtual ICollection<Feedback> Feedbacks { get; set; }
        //public virtual ICollection<OrderMasters> OrderMasters { get; set; }
    }
}
