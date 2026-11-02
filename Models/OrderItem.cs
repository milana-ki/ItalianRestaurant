using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItalianRestaurant.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtMoment { get; set; } 

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int DishId { get; set; }
        public Dish? Dish { get; set; }
    }
}