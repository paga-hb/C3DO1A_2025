using Microsoft.EntityFrameworkCore;
using Flixtube.History.Entities;

namespace Flixtube.History.Data;

public class ApplicationDbContext : DbContext
{
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