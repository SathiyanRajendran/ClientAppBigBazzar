using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class OrderMasters
    {
        [Key]
        public int OrderMasterId { get; set; }
        public int? OrderDate { get; set; }
        [Range(0, int.MaxValue)]
        public int? Total { get; set; }
        public string? CardNumber { get; set; }
        [Range(0, int.MaxValue)]
        public float? AmountPaid { get; set; }
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customers? Customers { get; set; }
        public virtual ICollection<OrderDetails>? OrderDetails { get; set; }
    }
}
