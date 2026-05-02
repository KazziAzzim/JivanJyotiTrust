using System.ComponentModel.DataAnnotations;
using JivanJyotiTrustApp.Models;

namespace JivanJyotiTrustApp.Areas.Admin.ViewModels;

public class ContentFormViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string SectionKey { get; set; } = string.Empty;

    public List<IFormFile> Files { get; set; } = [];

    public List<ContentFile> ExistingFiles { get; set; } = [];
}
