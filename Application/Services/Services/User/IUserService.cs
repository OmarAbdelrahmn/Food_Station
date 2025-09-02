using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Contracts.Users;

namespace SurvayBasket.Application.Services.User;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetUserProfile(string id);
    Task<Result> UpdateUserProfile(string id, UpdateUserProfileRequest request);
    Task<Result> ChangePassword(string id, ChangePasswordRequest request);
}
