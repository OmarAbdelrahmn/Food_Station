using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Contracts.Results;

namespace SurvayBasket.Application.Services.AddResults;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVotesAsynce(int pollId);
    Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsynce(int pollId);
    Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsynce(int pollId);
}
