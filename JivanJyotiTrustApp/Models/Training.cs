using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class Training
{
    public int Id { get; set; }

    [Required]
    public int CityId { get; set; }

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public City? City { get; set; }
    public ICollection<GalleryItem> GalleryItems { get; set; } = new List<GalleryItem>();
}
