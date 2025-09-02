namespace SurvayBasket.Infrastructure.EntitiesConfigrations;

public class AnswerConfigration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(p => new { p.Id, p.Content })
            .IsUnique();

    }
}

