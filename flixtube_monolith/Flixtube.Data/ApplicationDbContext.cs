using Microsoft.EntityFrameworkCore;
using Flixtube.Data.Entities;

namespace Flixtube.Data;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Video> Videos => Set<Video>();
    public virtual DbSet<ViewHistory> ViewHistorys => Set<ViewHistory>();

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

        modelBuilder.Entity<ViewHistory>()
        .HasKey(v => v.Id);

        modelBuilder.Entity<ViewHistory>().Property(v => v.Id)
        .HasColumnType("integer")
        .IsRequired()
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<ViewHistory>().Property(v => v.VideoId)
        .HasColumnType("nvarchar(125)")
        .IsRequired();

        modelBuilder.Entity<ViewHistory>().Property(v => v.ViewedAt)
        .HasColumnType("datetime2(7)")
        .IsRequired();

        // SeedData.Seed(modelBuilder);
    }
}