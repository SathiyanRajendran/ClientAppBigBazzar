using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Enter the Product Name")]
        [StringLength(50)]
        public string? ProductName { get; set; }
        public float? UnitPrice { get; set; }
        public string? ProductImage { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public int? ProductQuantity { get; set; }
        public virtual ICollection<OrderDetails>? OrderDetails { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<Carts>? Carts { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Categories? Categories { get; set; }
        public int? TraderId { get; set; }
        [ForeignKey("TraderId")]
        public virtual Traders? Traders { get; set; }
    }
}
