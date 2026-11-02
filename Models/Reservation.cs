using System.ComponentModel.DataAnnotations;

namespace ItalianRestaurant.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; } 

        [Required]
        [Range(1, 20)]
        public int GuestsCount { get; set; } 

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(250)]
        public string SpecialRequests { get; set; } = string.Empty; 

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}