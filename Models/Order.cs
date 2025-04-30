using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Login.Models
{
    public class Order
    {
        //[Key]
        //public Guid OrderId { get; set; } = Guid.NewGuid();

        //[ForeignKey("User")]
        //public Guid UserId { get; set; }  // Foreign key referencing Users table

        //public string? PizzaSize { get; set; }
        //public int? Quantity { get; set; }
        //public string? Toppings { get; set; } // You can use a more complex type if needed
        //public decimal? Price { get; set; }
        //public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Automatically set the order date

        //[ForeignKey("UserId")]
        //public virtual Users User { get; set; }  // Navigation property

        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        public Guid UserId { get; set; }  // Foreign key referencing Users table

        public string? PizzaSize { get; set; }
        public int? Quantity { get; set; }
        public string? Toppings { get; set; }
        public decimal? Price { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Automatically set the order date
    }
}
