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
}
