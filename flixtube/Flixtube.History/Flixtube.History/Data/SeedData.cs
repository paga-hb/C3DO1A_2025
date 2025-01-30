using Microsoft.EntityFrameworkCore;
using Flixtube.History.Entities;
using Flixtube.History.Repositories;

namespace Flixtube.History.Data;

public static class SeedData
{
    public static void Seed(ModelBuilder builder)
    {
        var history = new ViewHistory
        {
            Id = 1,
            VideoId = "5d9e690ad76fe06a3d7ae416",
            ViewedAt = DateTime.Now
        };

        builder.Entity<ViewHistory>().HasData(history);
    }

    public static async Task Initialize(IServiceProvider serviceProvider, bool seedDatabase)
    {
        // Make sure the database exists
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        while (true)
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                // Connection successful, so break out of while loop
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database creation attempt failed: {ex.Message}");
                await Task.Delay(5000); // Wait before retrying
            }
        }

        if(!seedDatabase)
        {
            // Don't seed the database
            return;
        }

        // Create a ViewHistory instance to seed the database
        var history = new ViewHistory
        {
            Id = 1,
            VideoId = "5d9e690ad76fe06a3d7ae416",
            ViewedAt = DateTime.Now
        };

        // Check if the database is already seeded
        var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        var v = await uow.ViewHistorys.FirstOrDefaultAsync(v => v.Id == history.Id);
        if(v != null)
        {
            // Database is already seeded, so return
            return;
        }

        // Database is not seeded, so seed it
        uow.ViewHistorys.Add(history);
        await uow.CompleteAsync();
    }
}