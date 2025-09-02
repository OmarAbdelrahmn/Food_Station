
namespace SurvayBasket.Infrastructure.EntitiesConfigrations;

public class VoteAnswerConfigration : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {

        builder.HasIndex(p => new { p.QuestionId, p.VoteId })
            .IsUnique();

    }
}

