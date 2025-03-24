using System.ComponentModel.DataAnnotations;

namespace JointTrips.Models
{
    public class User
    {
        public int UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public required string Username { get; set; }
        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public virtual ICollection<Trip> OwnedTrips { get; set; } = new List<Trip>();
        public virtual ICollection<Trip> TripsSignedUp { get; set; } = new List<Trip>();
    }
}
