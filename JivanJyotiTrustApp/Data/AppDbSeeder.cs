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
                new BannerItem { Title = "Empower Skills", ImagePath = "https://via.placeholder.com/1200x400?text=Banner+1" },
                new BannerItem { Title = "Build Futures", ImagePath = "https://via.placeholder.com/1200x400?text=Banner+2" },
                new BannerItem { Title = "Community Learning", ImagePath = "https://via.placeholder.com/1200x400?text=Banner+3" }
            );
        }

        if (!await context.ClassItems.AnyAsync())
        {
            context.ClassItems.AddRange(
                new ClassItem { Name = "Jewelry Making", ShortDescription = "Design beautiful handmade ornaments.", ImagePath = "https://via.placeholder.com/400x250?text=Jewelry+Making" },
                new ClassItem { Name = "Soap Making", ShortDescription = "Learn natural and decorative soap creation.", ImagePath = "https://via.placeholder.com/400x250?text=Soap+Making" },
                new ClassItem { Name = "Mehndi Class", ShortDescription = "Master traditional and modern mehndi styles.", ImagePath = "https://via.placeholder.com/400x250?text=Mehndi+Class" },
                new ClassItem { Name = "Art and Craft Class", ShortDescription = "Create engaging DIY art and craft projects.", ImagePath = "https://via.placeholder.com/400x250?text=Art+and+Craft" }
            );
        }

        await context.SaveChangesAsync();
    }
}
