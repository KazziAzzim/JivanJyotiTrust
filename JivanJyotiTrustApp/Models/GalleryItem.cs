using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class GalleryItem
{
    public int Id { get; set; }

    [Required]
    public int TrainingId { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public Training? Training { get; set; }
}
