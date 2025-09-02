
using SurveyBasket.Abstraction.Consts;
using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services.Roles;

public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbcontext dbcontext) : IRoleService
{
    private readonly RoleManager<ApplicationRole> roleManager = roleManager;
    private readonly ApplicationDbcontext dbcontext = dbcontext;

    public async Task<Result<RoleDetailsResponse>> addroleAsync(RoleRequest request)
    {
        var roleisexists = await roleManager.RoleExistsAsync(request.Name);

        if (roleisexists)
            return Result.Failure<RoleDetailsResponse>(RolesErrors.DaplicatedRole);

        var allowpermission = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowpermission).Any())
            return Result.Failure<RoleDetailsResponse>(RolesErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString(),

        };

        var result = await roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            var permissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });
            await dbcontext.AddRangeAsync(permissions);
            await dbcontext.SaveChangesAsync();

            var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, request.Permissions);

            return Result.Success(response);
        }

        var error = result.Errors.First();
        return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result<RoleDetailsResponse>> GetRoleByIdAsync(string RollId)
    {
        var role = await roleManager.FindByIdAsync(RollId);

        if (role == null)
            return Result.Failure<RoleDetailsResponse>(RolesErrors.NotFound);

        var permissions = await roleManager.GetClaimsAsync(role);

        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<RolesResponse>>> GetRolesAsync(bool? IncludeDisable = false)
    {
        var roles = await roleManager.Roles
            .Where(c => !c.IsDeleted || IncludeDisable == true)
            .ProjectToType<RolesResponse>()
            .ToListAsync();

        return Result.Success<IEnumerable<RolesResponse>>(roles);
    }

    public async Task<Result> ToggleStatusAsync(string RollId)
    {
        if (await roleManager.FindByIdAsync(RollId) is not { } role)
            return Result.Failure(RolesErrors.NotFound);

        role.IsDeleted = !role.IsDeleted;

        await roleManager.UpdateAsync(role);

        return Result.Success();
    }

    public async Task<Result> UpdateRoleAsync(string Id, RoleRequest request)
    {
        if (await roleManager.FindByIdAsync(Id) is not { } role)
            return Result.Failure(RolesErrors.NotFound);

        var roleisexists = await roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != Id);

        if (roleisexists)
            return Result.Failure(RolesErrors.DaplicatedRole);

        var allowpermission = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowpermission).Any())
            return Result.Failure(RolesErrors.InvalidPermissions);

        role.Name = request.Name;

        var result = await roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            var Currentpermissions = await dbcontext.RoleClaims
                .Where(c => c.RoleId == Id && c.ClaimType == Permissions.Type)
                .Select(c => c.ClaimValue)
                .ToListAsync();

            var newPermissions = request.Permissions
                .Except(Currentpermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

            var removedPermissions = Currentpermissions.Except(request.Permissions);

            await dbcontext.RoleClaims
                .Where(c => c.RoleId == Id && removedPermissions.Contains(c.ClaimValue))
                .ExecuteDeleteAsync();

            await dbcontext.AddRangeAsync(newPermissions);
            await dbcontext.SaveChangesAsync();

            return Result.Success();

        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));


    }
}
