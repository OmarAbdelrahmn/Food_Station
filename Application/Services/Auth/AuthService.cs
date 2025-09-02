using Microsoft.Extensions.Logging;
using SurvayBasket.Application.Abstraction;
using SurvayBasket.Application.Abstraction.Errors;
using SurvayBasket.Application.Contracts.Auth;
using SurvayBasket.Application.Services.Auth;
using SurvayBasket.Domain.Consts;
using SurvayBasket.Infrastructure.Authentication;
using SurvayBasket.Infrastructure.Dbcontext;
using SurvayBasket.Infrastructure.Helpers;

namespace SurvayBasket.Infrastructure.Services.Auth;

public class AuthService(
    UserManager<ApplicataionUser> manager,
    SignInManager<ApplicataionUser> signInManager
    , IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    AppDbcontext dbcontext) : IAuthService
{
    private readonly UserManager<ApplicataionUser> manager = manager;
    private readonly SignInManager<ApplicataionUser> signInManager = signInManager;
    private readonly IJwtProvider jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> logger = logger;
    private readonly IEmailSender emailSender = emailSender;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly AppDbcontext dbcontext = dbcontext;
    private readonly int RefreshTokenExpiryDays = 60;

    public async Task<Result<AuthResponse>> SingInAsync(AuthRequest request)
    {

        if (await manager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisable)
            return Result.Failure<AuthResponse>(UserErrors.Disableuser);

        //using user manager
        //var TruePassword = await manager.CheckPasswordAsync(user, request.Password);

        //if (!TruePassword)
        //    return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);



        //using signin manager
        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, true);

        if (result.Succeeded)
        {
            var userRoles = await manager.GetRolesAsync(user);
            var UserPermissions = await dbcontext.Roles
                .Join(dbcontext.RoleClaims, role => role.Id,
                claim => claim.RoleId,
                (role, claim) => new { role, claim })
                .Where(x => userRoles.Contains(x.role.Name!))
                .Select(x => x.claim.ClaimType)
                .Distinct()
                .ToListAsync();
            var (Token, ExpiresIn) = jwtProvider.GenerateToken(user, userRoles, UserPermissions!);

            var RefreshToken = GenerateRefreshToken();

            var RefreshExpiresIn = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);


            user.RefreshTokens.Add(new RefreshToken
            {
                Token = RefreshToken,
                ExpiresOn = RefreshExpiresIn,

            });

            await manager.UpdateAsync(user);

            var response = new AuthResponse(
                user.Id,
                user.Email!,
                user.FirstName,
                user.LastName,
                Token,
                ExpiresIn * 60,
                RefreshToken,
                RefreshExpiresIn
            );

            return Result.Success(response);
        }

        var error = result.IsNotAllowed ?
             UserErrors.EmailNotConfirmed :
             result.IsLockedOut ?
             UserErrors.userLockedout :
             UserErrors.InvalidCredentials;


        return Result.Failure<AuthResponse>(error);

    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(220));
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string RefreshToken)
    {
        var UserId = jwtProvider.ValidateToken(Token);

        if (UserId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var user = await manager.FindByIdAsync(UserId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisable)
            return Result.Failure<AuthResponse>(UserErrors.Disableuser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.userLockedout);


        var UserRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

        if (UserRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        UserRefreshToken.RevokedOn = DateTime.UtcNow;

        var userRoles = await manager.GetRolesAsync(user);
        var UserPermissions = await dbcontext.Roles
            .Join(dbcontext.RoleClaims, role => role.Id,
            claim => claim.RoleId,
            (role, claim) => new { role, claim })
            .Where(x => userRoles.Contains(x.role.Name!))
            .Select(x => x.claim.ClaimType)
            .Distinct()
            .ToListAsync();
        var (newToken, ExpiresIn) = jwtProvider.GenerateToken(user, userRoles, UserPermissions!);

        var newRefreshToken = GenerateRefreshToken();

        var RefreshExpiresIn = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);


        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = RefreshExpiresIn,

        });

        await manager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            newToken,
            ExpiresIn * 60,
            newRefreshToken,
            RefreshExpiresIn
        );

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string Token, string RefreshToken)
    {
        var UserId = jwtProvider.ValidateToken(Token);

        if (UserId is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        var user = await manager.FindByIdAsync(UserId);

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var UserRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

        if (UserRefreshToken is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        UserRefreshToken.RevokedOn = DateTime.UtcNow;

        await manager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var emailisex = await manager.Users.AnyAsync(i => i.Email == request.Email);

        if (emailisex)
            return Result.Failure(UserErrors.EmailAlreadyExist);

        var user = request.Adapt<ApplicataionUser>();

        var result = await manager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var code = await manager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            logger.LogInformation("Configration code : {code}", code);

            await sendemail(user, code);

            return Result.Success();
        }
        var errors = result.Errors.First();
        return Result.Failure(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ConfirmEmailAsync(ConfigrationEmailRequest request)
    {

        if (await manager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);


        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfermation);

        var code = request.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {

            return Result.Failure(UserErrors.InvalidCredentials);
        }


        var result = await manager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {

            await manager.AddToRoleAsync(user, DefaultRoles.Member);

            return Result.Success();
        }
        var errors = result.Errors.First();
        return Result.Failure(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ResendEmailAsync(ResendEmailRequest request)
    {
        if (await manager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();


        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfermation);

        var code = await manager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        logger.LogInformation("Configration code : {code}", code);

        //send email
        await sendemail(user, code);

        return Result.Success();
    }

    private async Task sendemail(ApplicataionUser user, string code)
    {
        var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailbody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string> {
                    { "{{name}}", user.FirstName } ,
                    { "{{action_url}}", $"{origin}/auth/emailconfigration?userid={user.Id}&code={code}" }

            });

        BackgroundJob.Enqueue(() => emailSender.SendEmailAsync(user.Email!, "Survay basket : Email configration", emailbody));
        await Task.CompletedTask;
    }

    private async Task sendchangepasswordemail(ApplicataionUser user, string code)
    {
        var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailbody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string> {
                    { "{{name}}", user.FirstName } ,
                    { "{{action_url}}", $"{origin}/auth/forgetpassword?email={user.Email}&code={code}" }

            });

        BackgroundJob.Enqueue(() => emailSender.SendEmailAsync(user.Email!, "Survay basket : change password", emailbody));
        await Task.CompletedTask;
    }

    public async Task<Result> ForgetPassordAsync(ForgetPasswordRequest request)
    {
        if (await manager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var code = await manager.GeneratePasswordResetTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        logger.LogInformation("Reset code : {code}", code);

        //send email
        await sendchangepasswordemail(user, code);

        return Result.Success();

    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await manager.FindByEmailAsync(request.Email);

        if (user == null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCredentials);

        IdentityResult identityResult;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

            identityResult = await manager.ResetPasswordAsync(user, code, request.Password);

        }
        catch (FormatException)
        {

            identityResult = IdentityResult.Failed(manager.ErrorDescriber.InvalidToken());
        }

        if (identityResult.Succeeded)
            return Result.Success();

        var error = identityResult.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }

}
