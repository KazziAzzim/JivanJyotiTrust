using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class BannerItem
{
    public int Id { get; set; }

    [StringLength(120)]
    public string? Title { get; set; }

    [Required]
    public string ImagePath { get; set; } = string.Empty;
}
