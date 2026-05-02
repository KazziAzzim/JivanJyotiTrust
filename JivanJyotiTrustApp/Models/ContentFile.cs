using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class ContentFile
{
    public int Id { get; set; }

    public int ContentId { get; set; }

    [Required, StringLength(260)]
    public string FileName { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string FilePath { get; set; } = string.Empty;

    public Content? Content { get; set; }
}
