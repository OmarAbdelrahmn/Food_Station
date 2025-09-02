using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;
public class ApplicataionUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? SecondPhone { get; set; }
    public bool IsDisable { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];

}
