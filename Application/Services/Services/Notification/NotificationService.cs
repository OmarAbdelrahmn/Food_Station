namespace SurveyBasket.Services.Notification;

public class NotificationService(
    ApplicationDbcontext dbcontext,
    UserManager<ApplicataionUser> manager,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender
    ) : INotificationService
{
    private readonly ApplicationDbcontext dbcontext = dbcontext;
    private readonly UserManager<ApplicataionUser> manager = manager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender emailSender = emailSender;

    public async Task SendNewPollNotification(int? PollId = null)
    {
        IEnumerable<Poll> polls = [];

        if (PollId.HasValue)
        {
            var poll = await dbcontext.Polls.SingleOrDefaultAsync(p => p.Id == PollId && p.IsPublished);
            polls = [poll!];
        }
        else
        {
            polls = await dbcontext.Polls
                .Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }
        //todo : send notification for members only

        var users = await manager.Users.ToListAsync();

        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                //todo : send notification
                var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

                var placeholder = new Dictionary<string, string>
                {
                    {"{{name}}", user.FirstName},
                    {"{{pollTill}}", poll.Title},
                    {"{{endDate}}", poll.EndsAt.ToString()},
                    {"{{url}}", $"{origin}/polls/start/{poll.Id}"}
                };
                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholder);
                await emailSender.SendEmailAsync(user.Email!, $"Survay Basket : New Poll {poll.Title}", body);
            }
        }
    }
}
