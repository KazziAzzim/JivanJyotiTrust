using System.ComponentModel.DataAnnotations;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Http;

namespace JivanJyotiTrustApp.Areas.Admin.ViewModels;

public class TrainingFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public int? CityId { get; set; }

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public List<IFormFile> Images { get; set; } = [];

    public List<City> CityOptions { get; set; } = [];
    public List<GalleryItem> ExistingGalleryItems { get; set; } = [];
}
