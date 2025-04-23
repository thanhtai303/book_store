using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        public virtual OrderHistory Order { get; set; }
        public virtual Book Book { get; set; }
    }
}