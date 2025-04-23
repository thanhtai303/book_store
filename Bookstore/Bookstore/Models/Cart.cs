using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }
}