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

        // one-to-many relationship between User (Owner) and Trip
        modelBuilder.Entity<Trip>()
            .HasOne(t => t.Owner)
            .WithMany(u => u.OwnedTrips)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        // many-to-many relationship for Trip participants
        modelBuilder.Entity<Trip>()
            .HasMany(t => t.Participants)
            .WithMany(u => u.TripsSignedUp)
            .UsingEntity(joinEntity => joinEntity.ToTable("TripParticipants"));
    }
}