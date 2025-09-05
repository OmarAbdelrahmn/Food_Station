using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace Domain.Entities;
public class ApplicataionUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? SecondPhone { get; set; }
    public bool IsDisable { get; set; }
    public Guid? ImageId { get; set; }
    public Image? Image { get; set; } = default!;
    public List<RefreshToken> RefreshTokens { get; set; } = [];

}
