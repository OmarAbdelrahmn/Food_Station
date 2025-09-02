using SurvayBasket.Domain.Consts;

namespace SurvayBasket.Infrastructure.EntitiesConfigrations;

public class RoleClaimConfigration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var permission = Permissions.GetAllPermissions();
        var addadminclaim = new List<IdentityRoleClaim<string>>();
        var addmemberclaim = new List<IdentityRoleClaim<string>>();

        //for (int i = 0; i < permission.Count; i++)
        //{
        //    addadminclaim.Add(new IdentityRoleClaim<string>
        //    {
        //        Id = i + 1,
        //        RoleId = DefaultRoles.AdminRoleId,
        //        ClaimType = Permissions.Type,
        //        ClaimValue = permission[i]
        //    });
        //}

        for (int i = 0; i < permission.Count; i++)
        {
            addmemberclaim.Add(new IdentityRoleClaim<string>
            {
                Id = i + 1,
                RoleId = DefaultRoles.MemberRoleId,
                ClaimType = Permissions.Type,
                ClaimValue = permission[i]
            });
        }

        //builder.HasData(
        //    addadminclaim

        //);

        builder.HasData(
            addmemberclaim

        );

    }
}

