using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class Customers
    {
        [Key]
        public int CustomerId { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Special Characters not allowed")]
        public string? CustomerName { get; set; }
        [Required(ErrorMessage = "EmailId is Required")]
        [DataType(DataType.EmailAddress)]
        public string? CustomerEmail { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "At least one uppercase, one lowercase, one digit, one special character and minimum eight in length")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]

        public string? Password { get; set; }
        [DataType(DataType.Password)]

        [Compare("Password", ErrorMessage = "Password and Confirmation Password do not match.")]
        [NotMapped]

        public string? ConfirmPassword { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]

        [StringLength(10, MinimumLength = 10)]
        public string? MobileNumber { get; set; }
        public string? CustomerCity { get; set; }
        [DataType(DataType.PostalCode)]
        [StringLength(6, MinimumLength = 6)]

        public string? Pincode { get; set; }

        public string? SecurityQuestion { get; set; }

        public string? Answer { get; set; }
        public bool? Status { get; set; }
        public string? SecurityCode { get; set; }
        public DateTime? CreationDate { get; set; }
        public virtual ICollection<Carts>? Carts { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<OrderMasters>? OrderMasters { get; set; }

    }
}
