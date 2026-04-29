using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class ClassItem
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    public string ImagePath { get; set; } = string.Empty;
}
