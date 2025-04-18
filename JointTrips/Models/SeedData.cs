﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JointTrips.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new JointTripsContext(
            serviceProvider.GetRequiredService<DbContextOptions<JointTripsContext>>()))
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string defaultEmail = "John@jointtrips.com";

            // Try to find an existing default user.
            var defaultUser = userManager.FindByEmailAsync(defaultEmail).GetAwaiter().GetResult();
            if (defaultUser == null)
            {
                // Create the default user if not found.
                defaultUser = new ApplicationUser
                {
                    Name = "John",
                    UserName = defaultEmail,
                    Email = defaultEmail
                };

                var createResult = userManager.CreateAsync(defaultUser, "Password1!").GetAwaiter().GetResult();
                if (!createResult.Succeeded)
                {
                    throw new Exception("Failed to create default user.");
                }
            }
            if (!context.Set<ApplicationUser>().Local.Any(u => u.Id == defaultUser.Id))
            {
                context.Attach(defaultUser);
            }
            // Look for any trips.
            if (context.Trips.Any())
            {
                return;   // DB has been seeded
            }
            context.Trips.AddRange(
                new Trip
                {
                    Title = "Sightseeing in Tokyo",
                    StartDate = DateTime.Parse("2025-05-22"),
                    EndDate = DateTime.Parse("2025-05-29"),
                    Price = 300.00M,
                    Location = "Tokyo",
                    Capacity = 20,
                    Description = "Enjoy a tour through Tokyo's highlights.",
                    Owners = new List<ApplicationUser> { defaultUser },
                    Participants = new List<ApplicationUser> { defaultUser }
                },
                new Trip
                {
                    Title = "Hiking Trip",
                    StartDate = DateTime.Parse("2025-07-24"),
                    EndDate = DateTime.Parse("2025-07-27"),
                    Price = 50.00M,
                    Location = "Tatra Mountains",
                    Capacity = 10,
                    Description = "A challenging hike through the Tatra Mountains.",
                    Owners = new List<ApplicationUser> { defaultUser },
                    Participants = new List<ApplicationUser> { defaultUser }
                },
                new Trip
                {
                    Title = "Camping",
                    StartDate = DateTime.Parse("2025-06-02"),
                    EndDate = DateTime.Parse("2025-06-06"),
                    Price = 20.00M,
                    Location = "Sierra National Forest",
                    Capacity = 8,
                    Description = "Spend a few nights under the stars.",
                    Owners = new List<ApplicationUser> { defaultUser },
                    Participants = new List<ApplicationUser> { defaultUser }
                },
                new Trip
                {
                    Title = "Stroll around Warsaw",
                    StartDate = DateTime.Parse("2025-04-24"),
                    EndDate = DateTime.Parse("2025-04-24"),
                    Price = 0.00M,
                    Location = "Warsaw",
                    Capacity = 7,
                    Description = "A relaxing walk through historic Warsaw.",
                    Owners = new List<ApplicationUser> { defaultUser },
                    Participants = new List<ApplicationUser> { defaultUser }
                }
            );
            context.SaveChanges();
        }
    }
}