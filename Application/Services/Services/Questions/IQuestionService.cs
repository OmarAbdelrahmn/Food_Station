using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Contracts.Questions;

namespace SurvayBasket.Application.Services.Questions;

public interface IQuestionService
{
    public Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request);
    public Task<Result<QuestionResponse>> GetAsync(int PollId, int Id);
    public Task<Result> UpdateAsync(int PollId, int Id, QuestionRequest request);
    public Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId);
    public Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int PollId, string UserId);

}
