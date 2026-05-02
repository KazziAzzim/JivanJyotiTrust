using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class Content
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string SectionKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ContentFile> ContentFiles { get; set; } = new List<ContentFile>();
}
