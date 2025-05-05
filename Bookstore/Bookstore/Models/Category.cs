using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace BookStore.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        [StringLength(10000)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}