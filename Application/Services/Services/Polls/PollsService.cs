using SurveyBasket.Services.Notification;

namespace SurveyBasket.Services.Polls;

public class PollsService(
    ApplicationDbcontext dbcontext,
    INotificationService notification
    ) : IPollsService
{
    private readonly INotificationService notification = notification;

    public async Task<Result<PollResponse>> CreatePollAsync(PollRequest Request)
    {
        var isexist = dbcontext.Polls.Any(x => x.Title == Request.Title);
        if (isexist)
            return Result.Failure<PollResponse>(PollsErrors.InvalidCredentials);


        var poll = Request.Adapt<Poll>();

        await dbcontext.Polls.AddAsync(poll);
        await dbcontext.SaveChangesAsync();
        var response = poll.Adapt<PollResponse>();
        return Result.Success(response);
    }

    public async Task<Result> DeletePollAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var poll = await dbcontext.Polls.FindAsync(pollId, cancellationToken);

        if (poll is null)
            return Result.Failure(PollsErrors.InvalidCredentials);

        dbcontext.Polls.Remove(poll);
        dbcontext.SaveChanges();

        return Result.Success();
    }

    public async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync()
    {
        var response = await dbcontext.Polls
            .AsNoTracking()
            .Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .ProjectToType<PollResponse>()
            .ToListAsync();

        return Result.Success<IEnumerable<PollResponse>>(response);
    }

    public async Task<Result<PollResponse>> GetPollByIdAsync(int pollId)
    {
        var poll = await dbcontext.Polls.FindAsync(pollId);

        var response = poll.Adapt<PollResponse>();

        return poll is not null ?
            Result.Success(response) :
            Result.Failure<PollResponse>(PollsErrors.InvalidCredentials);
    }

    public async Task<Result<IEnumerable<PollResponse>>> GetPollsAsync()

    {
        var response = await dbcontext.Polls.AsNoTracking().ProjectToType<PollResponse>().ToListAsync();


        return Result.Success<IEnumerable<PollResponse>>(response);
    }

    public async Task<Result> ToggleStatus(int Id)
    {
        var poll = await dbcontext.Polls.FindAsync(Id);
        if (poll is null)
            return Result.Failure(PollsErrors.InvalidCredentials);

        poll.IsPublished = !poll.IsPublished;
        dbcontext.Polls.Update(poll);
        dbcontext.SaveChanges();

        if (poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => notification.SendNewPollNotification(poll.Id));

        return Result.Success();


    }

    public async Task<Result<PollResponse>> UpdatePollAsync(int pollId, PollRequest pollRequest)
    {
        var isexist = dbcontext.Polls.Any(x => x.Title == pollRequest.Title && x.Id != pollId);

        if (isexist)
            return Result.Failure<PollResponse>(PollsErrors.InvalidCredentials);


        var poll = await dbcontext.Polls.FindAsync(pollId);
        if (poll is null)
            return Result.Failure<PollResponse>(PollsErrors.InvalidCredentials);

        poll.Summary = pollRequest.Summary;
        poll.Title = pollRequest.Title;
        //poll.IsPublished = pollRequest.IsPublished;
        poll.EndsAt = pollRequest.EndsAt;
        poll.StartsAt = pollRequest.StartsAt;


        dbcontext.Polls.Update(poll);
        dbcontext.SaveChanges();

        var res = poll.Adapt<PollResponse>();

        return Result.Success(res);
    }
}
