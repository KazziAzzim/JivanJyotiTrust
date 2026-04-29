using Microsoft.AspNetCore.Identity;

namespace JivanJyotiTrustApp.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
