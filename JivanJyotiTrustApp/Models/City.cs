using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class City
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Training> Trainings { get; set; } = new List<Training>();
}
