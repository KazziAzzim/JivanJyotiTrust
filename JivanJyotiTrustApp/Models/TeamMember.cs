using System.ComponentModel.DataAnnotations;

namespace JivanJyotiTrustApp.Models;

public class TeamMember
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Designation { get; set; } = string.Empty;
}
