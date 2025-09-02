using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Abstraction.Consts;

namespace Application.EntitiesConfigrations;

public class UserConfigration : IEntityTypeConfiguration<ApplicataionUser>
{
    public void Configure(EntityTypeBuilder<ApplicataionUser> builder)
    {
        builder
            .OwnsMany(p => p.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");


        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(new ApplicataionUser
        {
            Id = DefaultUsers.AdminId,
            UserName = DefaultUsers.AdminEmail,
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<ApplicataionUser>().HashPassword(null!, "P@ssword1234"),
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            FirstName = "Survay Basket",
            LastName = "Admin"
        });

    }
}

