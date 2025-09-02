using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Contracts.Votes;

namespace SurvayBasket.Application.Services.Votes;

public interface IVotesService
{
    Task<Result> AddVote(int PollId, string UserId, VotesRequest request);
}
