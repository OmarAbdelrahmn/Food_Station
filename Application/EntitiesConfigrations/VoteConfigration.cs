
namespace SurvayBasket.Infrastructure.EntitiesConfigrations;

public class VoteConfigration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {


        builder.HasIndex(p => new { p.PollId, p.UserId })
            .IsUnique();

    }
}

