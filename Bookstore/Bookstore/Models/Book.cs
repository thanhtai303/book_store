namespace BookStore.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int YearOfPublication { get; set; }
        public string LongDescription { get; set; }
        public int CategoryId { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public string Publisher { get; set; }
        public string ImageS { get; set; }
        public string ImageM { get; set; }
        public string ImageL { get; set; }

<<<<<<< Updated upstream
        public DateTime CreatedDate { get; set; }

        // Foreign key for Category
        public int? CategoryId { get; set; }

        // Navigation property for Category
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
=======
        public Category Category { get; set; }
>>>>>>> Stashed changes
    }
}