using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ClassItem> ClassItems => Set<ClassItem>();
    public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();
    public DbSet<BannerItem> BannerItems => Set<BannerItem>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Training> Trainings => Set<Training>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<City>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<Training>()
            .HasOne(x => x.City)
            .WithMany(x => x.Trainings)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GalleryItem>()
            .HasOne(x => x.Training)
            .WithMany(x => x.GalleryItems)
            .HasForeignKey(x => x.TrainingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
