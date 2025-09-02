namespace SurvayBasket.Infrastructure.EntitiesConfigrations;

public class QuestionConfigration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(p => new { p.PollsId, p.Content })
            .IsUnique();
    }
}

