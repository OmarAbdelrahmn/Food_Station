using Application.Contracts.Users;
using Application.Extensions;
using Application.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Food_Station.Controllers;
[Route("me")]
[ApiController]
[Authorize(Roles = $"{DefaultRoles.Member},{DefaultRoles.Admin}")]
public class UserController(IUserService service) : ControllerBase
{
    private readonly IUserService service = service;

    [HttpGet("")]
    public async Task<IActionResult> ShowUserProfile()
    {
        var result = await service.GetUserProfile(User.GetUserId()!);

        return Ok(result.Value);
    }

    [HttpPut("info")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request)
    {
        var result = await service.UpdateUserProfile(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await service.ChangePassword(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImagesAsync([FromForm] UpdoadImagessRequest request)
    {
        var Id = await service.UpoadImage(User.GetUserId()!, request.Image);

        return Created();
    }
    public record UpdoadImagessRequest
    (
        IFormFile Image
    );

    [HttpGet("image-stream")]
    public async Task<IActionResult> Stream()
    {
        var (filestream, contenttype, filename) = await service.FileStream(User.GetUserId()!);

        return filestream is null ?
            NoContent() :
            File(filestream, contenttype, filename, enableRangeProcessing: true);
    }

    [HttpDelete("delete-image")]
    public async Task<IActionResult> Delete()
    {
        var response = await service.DeleteImage(User.GetUserId()!);

        return response.IsSuccess ?
            NoContent() :
            response.ToProblem();
    }
}
