﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace JointTrips.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        [MaxLength(100)]
        [Required]
        public string? Name { get; set; }

        public virtual ICollection<Trip> OwnedTrips { get; set; } = new List<Trip>();
        public virtual ICollection<Trip> TripsSignedUp { get; set; } = new List<Trip>();
    }
}
