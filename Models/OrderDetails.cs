using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products? Products { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public float? ProductRate { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public float? ProductQuantity { get; set; }
        public int? OrderMasterId { get; set; }
        [ForeignKey("OrderMasterId")]
        public virtual OrderMasters? OrderMasters { get; set; }
    }
}
