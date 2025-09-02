using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Contracts.Roles;

namespace SurvayBasket.Application.Services.Roles;

public interface IRoleService
{
    Task<Result<IEnumerable<RolesResponse>>> GetRolesAsync(bool? IncludeDisable = false);
    Task<Result<RoleDetailsResponse>> GetRoleByIdAsync(string RollId);
    Task<Result> ToggleStatusAsync(string RollId);
    Task<Result<RoleDetailsResponse>> addroleAsync(RoleRequest request);
    Task<Result> UpdateRoleAsync(string Id, RoleRequest request);
}
