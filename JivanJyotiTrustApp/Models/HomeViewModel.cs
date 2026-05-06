namespace JivanJyotiTrustApp.Models;

public class HomeViewModel
{
    public List<BannerItem> Banners { get; set; } = [];
    public List<ClassItem> Classes { get; set; } = [];
    public StatsViewModel Stats { get; set; } = new();
    public List<TeamMember> TeamMembers { get; set; } = [];
}
