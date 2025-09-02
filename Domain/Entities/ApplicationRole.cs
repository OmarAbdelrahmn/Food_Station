namespace Domain.Entities;
public class ApplicationRole : Microsoft.AspNetCore.Identity.IdentityRole
{
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
}
