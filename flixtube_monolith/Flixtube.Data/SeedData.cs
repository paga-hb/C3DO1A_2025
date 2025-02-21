using Microsoft.EntityFrameworkCore;
using Flixtube.Data.Entities;
using Flixtube.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Flixtube.Data;

public static class SeedData
{
    public static void Seed(ModelBuilder builder)
    {
        var video = new Video
        {
            Id = "5d9e690ad76fe06a3d7ae416",
            Name = "SampleVideo_1280x720_1mb.mp4"
        };

        builder.Entity<Video>().HasData(video);

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

        //
        // VIDEO
        //

        // Create a Video instance to seed the database
        var video = new Video
        {
            Id = "5d9e690ad76fe06a3d7ae416",
            Name = "SampleVideo_1280x720_1mb.mp4"
        };

        // Check if the database is already seeded
        var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        var v = await uow.Videos.FirstOrDefaultAsync(v => v.Id == video.Id);
        if(v == null)
        {
            // Database is not seeded, so seed it
            uow.Videos.Add(video);
            await uow.CompleteAsync();
        }

        //
        // VIEW HISTORY
        //

        // Create a ViewHistory instance to seed the database
        var history = new ViewHistory
        {
            Id = 1,
            VideoId = "5d9e690ad76fe06a3d7ae416",
            ViewedAt = DateTime.Now
        };

        // Check if the database is already seeded
        uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        var vh = await uow.ViewHistorys.FirstOrDefaultAsync(vh => vh.Id == history.Id);
        if(vh == null)
        {
            // Database is not seeded, so seed it
            uow.ViewHistorys.Add(history);
            await uow.CompleteAsync();
        }
    }
}