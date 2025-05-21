namespace BookStore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
<<<<<<< Updated upstream

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        [StringLength(10000)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Book> Books { get; set; }
=======
        public string Name { get; set; }
        public List<Book> Books { get; set; }
>>>>>>> Stashed changes
    }
}