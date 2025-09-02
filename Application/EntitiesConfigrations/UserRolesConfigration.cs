using SurvayBasket.Domain.Consts;

namespace SurvayBasket.Infrastructure.EntitiesConfigrations;

public class UserRolesConfigration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {

        builder.HasData(
            new IdentityUserRole<string>
            {
                UserId = DefaultUsers.AdminId,
                RoleId = DefaultRoles.AdminRoleId
            }


        );

    }
}

