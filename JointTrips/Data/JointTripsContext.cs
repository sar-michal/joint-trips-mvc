using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using JointTrips.Models;


public class JointTripsContext : IdentityDbContext<ApplicationUser>
{
    public JointTripsContext(DbContextOptions<JointTripsContext> options)
        : base(options)
    {
    }

    public DbSet<Trip> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // many-to-many relationship for trip Owners
        modelBuilder.Entity<Trip>()
            .HasMany(t => t.Owners)
            .WithMany(u => u.OwnedTrips)
            .UsingEntity(joinEntity => joinEntity.ToTable("TripOwners"));

        // many-to-many relationship for Trip participants
        modelBuilder.Entity<Trip>()
            .HasMany(t => t.Participants)
            .WithMany(u => u.TripsSignedUp)
            .UsingEntity(joinEntity => joinEntity.ToTable("TripParticipants"));
    }
}