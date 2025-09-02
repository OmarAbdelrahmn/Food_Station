namespace SurveyBasket.Services.Polls;

public interface IPollsService
{
    Task<Result<PollResponse>> CreatePollAsync(PollRequest pollRequest);
    Task<Result<PollResponse>> GetPollByIdAsync(int pollId);
    Task<Result<IEnumerable<PollResponse>>> GetPollsAsync();
    Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync();
    Task<Result<PollResponse>> UpdatePollAsync(int pollId, PollRequest pollRequest);
    Task<Result> DeletePollAsync(int pollId, CancellationToken cancellationToken = default);

    Task<Result> ToggleStatus(int Id);
}
