using Application.Abstraction.Errors;
using Application.Contracts.Roles;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Contracts.Roles;


namespace Application.Services.Roles;

public class RoleService(RoleManager<ApplicationRole> roleManager, AppDbcontext dbcontext) : IRoleService
{
    private readonly RoleManager<ApplicationRole> roleManager = roleManager;
    private readonly AppDbcontext dbcontext = dbcontext;

    public async Task<Result<RoleDetailsResponse>> AddroleAsync(RoleRequest request)
    {
        var roleisexists = await roleManager.RoleExistsAsync(request.Name);

        if (roleisexists)
            return Result.Failure<RoleDetailsResponse>(RolesErrors.DaplicatedRole);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString(),

        };

        var result = await roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
          
            await dbcontext.SaveChangesAsync();

            var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted);

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


        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted);

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

        role.Name = request.Name;

        var result = await roleManager.UpdateAsync(role);

        if (result.Succeeded)
            return Result.Success();
        
        return Result.Success();

    }



}

