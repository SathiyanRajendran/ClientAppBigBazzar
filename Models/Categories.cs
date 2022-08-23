using System.ComponentModel.DataAnnotations;

namespace ClientAppBigBazzar.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string? CategoryName { get; set; }

        public virtual ICollection<Products>? Products { get; set; }
    }
}
