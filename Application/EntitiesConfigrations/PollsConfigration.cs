namespace SurvayBasket.Infrastructure.EntitiesConfigrations;

public class PollsConfigration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(55);

        builder.Property(p => p.Summary)
            .IsRequired()
            .HasMaxLength(500);

    }
}

