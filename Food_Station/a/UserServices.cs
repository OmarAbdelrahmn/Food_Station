using Application.Contracts.Users;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurvayBasket.Application.Abstraction;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Image = Domain.Entities.Image;

namespace Application.Services.User;

public class UserServices(UserManager<ApplicataionUser> manager,
                          AppDbcontext dbcontext,
                          IWebHostEnvironment webHostEnvironment) : IUserService

{
    private readonly UserManager<ApplicataionUser> manager = manager;
    private readonly AppDbcontext dbcontext = dbcontext;
    private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;

    private readonly string Imageepath = $"{webHostEnvironment.WebRootPath}/images";


    public async Task<Result> ChangePassword(string id, ChangePasswordRequest request)
    {
        var user = await manager.FindByIdAsync(id);

        var result = await manager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassord);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result<UserProfileResponse>> GetUserProfile(string id)
    {
        var user = await manager.Users
            .Where(i => i.Id == id)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();
        

        return Result.Success(user);
    }

    public async Task<Result> UpdateUserProfile(string id, UpdateUserProfileRequest request)
    {
        //var user = await manager.FindByIdAsync(id);

        //user = request.Adapt(user);

        //await manager.UpdateAsync(user!);
        await manager.Users
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(set =>
            set.SetProperty(x => x.FirstName, request.FirstName)
               .SetProperty(x => x.LastName, request.LastName));

        return Result.Success();
    }

    public async Task<Guid> UpoadImage(string id, IFormFile file)
    {
        var user = await manager.FindByIdAsync(id);

        var randomefilename = Path.GetRandomFileName();

        var uploadedfile = new Image
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            StoredFileName = randomefilename,
            FileExtenstions = Path.GetExtension(file.FileName),
            TypeId = id,
            Imagetype = Image.ImageType.User
        };

        
        var path = Path.Combine(Imageepath, uploadedfile.StoredFileName);

        using var stream = File.Create(path);

        await file.CopyToAsync(stream);


        await dbcontext.AddAsync(uploadedfile);
        await dbcontext.SaveChangesAsync();


        user!.ImageId = uploadedfile.Id;
        await manager.UpdateAsync(user!);


        await dbcontext.SaveChangesAsync();

        return uploadedfile.Id;
    }

    public async Task<Result> DeleteImage(string id)
    {
        var file = await dbcontext.Images.Where(c => c.TypeId == id && c.Imagetype == Image.ImageType.User).SingleOrDefaultAsync();
        if (file == null)
            return Result.Failure(new Error("No.Image", "No Image To Delete", StatusCodes.Status404NotFound));

        var path = Path.Combine(Imageepath, file.StoredFileName);
        if (File.Exists(path))
            File.Delete(path);

        var deletefile = await dbcontext.Images
            .Where(i => i.TypeId == id && i.Imagetype == Image.ImageType.User)
            .ExecuteDeleteAsync();

        if (deletefile == 0)
            return Result.Failure(new Error("someThingWrong", "SomeThingWentWrong", StatusCodes.Status404NotFound));

        var user = await manager.FindByIdAsync(id);

        user!.ImageId = null;

        await manager.UpdateAsync(user);

        await dbcontext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<(FileStream? fileStream, string contentType, string fileName)> FileStream(string id)
    {
        var user = await manager.FindByIdAsync(id);

        var Id = user!.ImageId;

        var file = await dbcontext.Images.Where(c => c.TypeId == id && c.Imagetype == Image.ImageType.User).SingleOrDefaultAsync();

        if (file == null)
            return (null, string.Empty, string.Empty);

        var path = Path.Combine(Imageepath, file.StoredFileName);

        var filestream = File.OpenRead(path);

        return (filestream, file.ContentType, file.FileName);
    }
}
