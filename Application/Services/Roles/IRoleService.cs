using Application.Contracts.Roles;
using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Contracts.Roles;

namespace Application.Services.Roles;

public interface IRoleService
{
    Task<Result<IEnumerable<RolesResponse>>> GetRolesAsync(bool? IncludeDisable = false);
    Task<Result<RoleDetailsResponse>> GetRoleByIdAsync(string RollId);
    Task<Result> ToggleStatusAsync(string RollId);
    Task<Result<RoleDetailsResponse>> AddroleAsync(RoleRequest request);
    Task<Result> UpdateRoleAsync(string Id, RoleRequest request);
}
