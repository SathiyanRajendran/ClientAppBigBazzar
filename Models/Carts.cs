using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class Carts
    {
        [Key]
        public int CartId { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products? Products { get; set; }
        //public virtual ICollection<Products>? Product { get; set; }
        [Required]
        [Range(0, 7, ErrorMessage = "You are Not allowed more Products added to Cart")]
        public int? ProductQuantity { get; set; }
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customers? Customers { get; set; }

    }
}
