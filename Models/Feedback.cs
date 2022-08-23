using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientAppBigBazzar.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public string? Comment { get; set; }
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customers? Customers { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products? Products { get; set; }
    }
}
