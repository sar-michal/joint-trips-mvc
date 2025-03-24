using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JointTrips.Models;

public class Trip
{
    public int Id { get; set; }

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Range(0, 10000)]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(19, 2)")]
    public decimal Price { get; set; }

    [RegularExpression(@"^[\p{Lu}]+[\p{L}\s]*$")]
    [Required]
    [StringLength(30)]
    public string? Location { get; set; }

    [Range(1, 10000)]
    [Required]
    public int Capacity { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; } // Data

    public int OwnerId { get; set; }
    public virtual User? Owner { get; set; }
    public virtual ICollection<User> Participants { get; set; } = new List<User>();
}