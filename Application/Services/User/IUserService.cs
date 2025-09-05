using Application.Contracts.Users;
using Microsoft.AspNetCore.Http;
using SurvayBasket.Application.Abstraction;

namespace Application.Services.User;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetUserProfile(string id);
    Task<Result> UpdateUserProfile(string id, UpdateUserProfileRequest request);
    Task<Result> ChangePassword(string id, ChangePasswordRequest request);
    Task<Guid> UpoadImage(string id, IFormFile image);
    Task<Result> DeleteImage(string id);
    Task<(FileStream? fileStream, string contentType, string fileName)> FileStream(string id);
}
