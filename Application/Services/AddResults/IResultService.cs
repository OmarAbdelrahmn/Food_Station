using SurveyBasket.Contracts.Results;

namespace SurveyBasket.Services.AddResults;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVotesAsynce(int pollId);
    Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsynce(int pollId);
    Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsynce(int pollId);
}
