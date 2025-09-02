using SurveyBasket.Contracts.Votes;

namespace SurveyBasket.Services.Votes;

public interface IVotesService
{
    Task<Result> AddVote(int PollId, string UserId, VotesRequest request);
}
