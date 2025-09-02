using Application.Contracts.Users;
using SurvayBasket.Application.Abstraction;
using TechSpire.APi.Contracts.Users;

namespace Application.Services.Admin;

public interface IAdminService
{
    Task<IEnumerable<UserResponse>> GetAllUsers();
    Task<Result<UserResponse>> GetUserAsync(string Id);
    Task<Result<UserResponse>> AddUserAsync(CreateUserRequest request);
    Task<Result> UpdateUserAsync(string UserId, UpdateUserRequest request);
    Task<Result> ToggleStatusAsync(string UserId);
    Task<Result> EndLockOutAsync(string UserId);
}
