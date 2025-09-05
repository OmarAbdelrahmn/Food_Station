using Application.Contracts.Users;
using SurvayBasket.Application.Abstraction;

namespace Application.Services.User;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetUserProfile(string id);
    Task<Result> UpdateUserProfile(string id, UpdateUserProfileRequest request);
    Task<Result> ChangePassword(string id, ChangePasswordRequest request);
}
