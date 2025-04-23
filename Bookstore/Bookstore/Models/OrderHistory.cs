using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BookStore.Models
{
    public class OrderHistory
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal TotalPrice { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
