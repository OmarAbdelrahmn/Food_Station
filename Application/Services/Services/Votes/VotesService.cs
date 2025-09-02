using SurveyBasket.Contracts.Votes;

namespace SurveyBasket.Services.Votes;

public class VotesService(ApplicationDbcontext dbcontext) : IVotesService
{
    public async Task<Result> AddVote(int PollId, string UserId, VotesRequest request)
    {
        var isExist = await dbcontext.Votes.AnyAsync(r => r.Id == PollId && r.UserId == UserId);

        if (isExist)
            return Result.Failure(VotesErrors.DaplicatedVote);

        var PollIsExist = await dbcontext.Polls.AnyAsync(x => x.Id == PollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));

        if (!PollIsExist)
            return Result.Failure(PollsErrors.NotFound);

        var questionId = await dbcontext.Questions.Where(x => x.PollsId == PollId && x.IsActive).Select(x => x.Id).ToListAsync();

        if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(questionId))
            return Result.Failure(QuestionErrors.InvalidQuestions);

        var Vote = new Vote()
        {
            PollId = PollId,
            UserId = UserId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await dbcontext.AddAsync(Vote);
        await dbcontext.SaveChangesAsync();

        return Result.Success();
    }
}
