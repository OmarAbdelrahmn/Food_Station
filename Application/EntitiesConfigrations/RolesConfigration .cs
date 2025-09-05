using Application.Abstraction.Consts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.EntitiesConfigrations;

public class RolesConfigration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {

        builder.HasData(
            [
                new ApplicationRole
                {
                    Id = DefaultRoles.AdminRoleId,
                    Name = DefaultRoles.Admin,
                    ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                    NormalizedName = DefaultRoles.Admin.ToUpper(),
                    IsDefault = false,
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.MemberRoleId,
                    Name = DefaultRoles.Member,
                    ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                    NormalizedName = DefaultRoles.Member.ToUpper(),
                    IsDefault = true,
                    IsDeleted = false
                }
            ]
        );

    }
}

