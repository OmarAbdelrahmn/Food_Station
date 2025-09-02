using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Abstraction.Errors;
using SurvayBasket.Application.Contracts.Results;
using SurvayBasket.Application.Services.AddResults;
using SurvayBasket.Infrastructure.Dbcontext;

namespace SurvayBasket.Infrastructure.Services.AddResults;

public class ResultService(AppDbcontext dbcontext) : IResultService
{
    private readonly AppDbcontext dbcontext = dbcontext;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsynce(int pollId)
    {
        var poll = await dbcontext.Polls
            .Where(p => p.Id == pollId)
            .Select(x => new PollVotesResponse(
                x.Title,
                x.Votes.Select(v => new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmitAt,
                    v.VoteAnswers
                    .Select(a => new QuestionAnswerResponse(
                        a.Question.Content,
                        a.Answer.Content
                    ))
                  ))
                ))
                  .SingleOrDefaultAsync();

        return poll is null
            ? Result.Failure<PollVotesResponse>(PollsErrors.NotFound)
            : Result.Success(poll);

    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsynce(int pollId)
    {
        var pollIsExist = await dbcontext.Polls.AnyAsync(r => r.Id == pollId);

        if (!pollIsExist)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollsErrors.NotFound);

        var votesperday = await dbcontext.Votes
            .Where(r => r.PollId == pollId)
            .GroupBy(r => new { Date = DateOnly.FromDateTime(r.SubmitAt) })
            .Select(r => new VotesPerDayResponse(r.Key.Date, r.Count()))
            .ToListAsync();

        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesperday);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsynce(int pollId)
    {
        var pollIsExist = await dbcontext.Polls.AnyAsync(r => r.Id == pollId);

        if (!pollIsExist)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollsErrors.NotFound);

        var votesperday = await dbcontext.VoteAnswers
            .Where(x => x.Vote.PollId == pollId)
            .Select(x => new VotesPerQuestionResponse(
                x.Question.Content,
                x.Question.VoteAnswer
                    .GroupBy(x => new { AnswerId = x.Answer.Id, AnswerContent = x.Answer.Content })
                    .Select(g => new VotesPerAnswerResponse(
                        g.Key.AnswerContent,
                        g.Count()
                        ))
                ))
            .ToListAsync();

        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesperday);

    }

}
