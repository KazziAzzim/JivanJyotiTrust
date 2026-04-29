using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Data;

public static class AppDbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminRole = "Admin";
        const string adminEmail = "Admin@gmail.com";
        const string adminPassword = "Admin@123";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
        else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }

        if (!await context.BannerItems.AnyAsync())
        {
            context.BannerItems.AddRange(
                new BannerItem { Title = "Empower Skills", ImagePath = "/images/banner-1.svg" },
                new BannerItem { Title = "Build Futures", ImagePath = "/images/banner-2.svg" },
                new BannerItem { Title = "Community Learning", ImagePath = "/images/banner-3.svg" }
            );
        }

        if (!await context.ClassItems.AnyAsync())
        {
            context.ClassItems.AddRange(
                new ClassItem { Name = "Jewelry Making", ShortDescription = "Design beautiful handmade ornaments.", ImagePath = "/images/class-jewelry.svg" },
                new ClassItem { Name = "Soap Making", ShortDescription = "Learn natural and decorative soap creation.", ImagePath = "/images/class-soap.svg" },
                new ClassItem { Name = "Mehndi Class", ShortDescription = "Master traditional and modern mehndi styles.", ImagePath = "/images/class-mehndi.svg" },
                new ClassItem { Name = "Art and Craft Class", ShortDescription = "Create engaging DIY art and craft projects.", ImagePath = "/images/class-art.svg" }
            );
        }

        if (!await context.GalleryItems.AnyAsync())
        {
            context.GalleryItems.AddRange(
                new GalleryItem { Title = "Workshop 1", ImagePath = "/images/gallery-1.svg" },
                new GalleryItem { Title = "Workshop 2", ImagePath = "/images/gallery-2.svg" },
                new GalleryItem { Title = "Workshop 3", ImagePath = "/images/gallery-3.svg" },
                new GalleryItem { Title = "Workshop 4", ImagePath = "/images/gallery-4.svg" }
            );
        }

        await context.SaveChangesAsync();
    }
}
