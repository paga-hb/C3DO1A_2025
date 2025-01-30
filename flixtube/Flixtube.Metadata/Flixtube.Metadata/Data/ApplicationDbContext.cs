using Microsoft.EntityFrameworkCore;
using Flixtube.Metadata.Entities;

namespace Flixtube.Metadata.Data;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Video> Videos => Set<Video>();

    public ApplicationDbContext() {}

    public ApplicationDbContext(DbContextOptions options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Video>()
        .HasKey(v => v.Id);

        modelBuilder.Entity<Video>().Property(v => v.Id)
        .HasColumnType("nvarchar(125)")
        .IsRequired();
        // .ValueGeneratedOnAdd();

        modelBuilder.Entity<Video>().Property(v => v.Name)
        .HasColumnType("nvarchar(125)")
        .HasMaxLength(125)
        .IsRequired();

        // SeedData.Seed(modelBuilder);
    }
}