//using Food_Station;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using SurvayBasket.Application.Services.AddResults;
//using SurvayBasket.Application.Services.Admin;
//using SurvayBasket.Application.Services.Auth;
//using SurvayBasket.Application.Services.Notification;
//using SurvayBasket.Application.Services.Polls;
//using SurvayBasket.Application.Services.Questions;
//using SurvayBasket.Application.Services.Roles;
//using SurvayBasket.Application.Services.User;
//using SurvayBasket.Application.Services.Votes;
//using SurvayBasket.Infrastructure.Authentication;
//using SurvayBasket.Infrastructure.Authentication.Filters;
////using SurvayBasket.Infrastructure.Dbcontext;
//using SurvayBasket.Infrastructure.Services.AddResults;
//using SurvayBasket.Infrastructure.Services.Admin;
//using SurvayBasket.Infrastructure.Services.Auth;
//using SurvayBasket.Infrastructure.Services.Notification;
//using SurvayBasket.Infrastructure.Services.Polls;
//using SurvayBasket.Infrastructure.Services.Questions;
//using SurvayBasket.Infrastructure.Services.Roles;
//using SurvayBasket.Infrastructure.Services.User;
//using SurvayBasket.Infrastructure.Services.Votes;
////using SurvayBasket.Infrastructure.Settings;
////using Microsoft.OpenApi.Models;


using Application;
using Domain.Entities;
using SurvayBasket.Infrastructure.Authentication;

namespace Food_Station;
public static class InfraDependencies
{
    // This class is used to group all the dependencies related to the infrastructure layer.

    public static IServiceCollection AddDep(this IServiceCollection Services, IConfiguration configuration)
    {
        Services.AddControllers();

        Services.AddEndpointsApiExplorer();
        Services.AddHttpContextAccessor();
        //Services.AddScoped<IPollsService, PollsService>();
        //Services.AddScoped<IAdminService, AdminService>();
        //Services.AddScoped<IRoleService, RoleService>();
        //Services.AddScoped<IUserService, UserServices>();
        //Services.AddScoped<INotificationService, NotificationService>();
        //Services.AddScoped<IResultService, ResultService>();
        //Services.AddScoped<IVotesService, VotesService>();
        //Services.AddScoped<IQuestionService, QuestionService>();
        //Services.AddScoped<IEmailSender, EmailService>();
        //Services.AddScoped<IAuthService, AuthService>();
        //Services.AddScoped<IJwtProvider, JwtProvider>();


        Services.AddProblemDetails();



        Services.AddAuth(configuration)
                .AddMappester()
                .AddFluentValidation()
                //.AddSwagger()
                .AddDatabase(configuration)
                .AddCORS()
                //.AddHangfire(configuration)
                ;


        return Services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection Services)
    {
        Services
            //.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return Services;
    }
    public static IServiceCollection AddMappester(this IServiceCollection Services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        Services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return Services;
    }
    public static IServiceCollection AddDatabase(this IServiceCollection Services, IConfiguration c)
    {
        var ConnectionString = c.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string is not found in the configuration file");

        Services.AddDbContext<AppDbcontext>(options =>
            options.UseSqlServer(ConnectionString));

        return Services;
    }
    public static IServiceCollection AddAuth(this IServiceCollection Services, IConfiguration configuration)
    {
        //    Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        //    Services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        Services.AddIdentity<ApplicataionUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppDbcontext>()
            .AddDefaultTokenProviders();

        Services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        //Services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        var Jwtsetting = configuration.GetSection("Jwt").Get<JwtOptions>();

        Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {


                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = Jwtsetting?.Audience,
                ValidIssuer = Jwtsetting?.Issuer,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwtsetting?.Key!))
            };
        });
        Services.Configure<IdentityOptions>(options =>
        {
            // Default Lockout settings.
            //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //options.Lockout.MaxFailedAccessAttempts = 5;
            //options.Lockout.AllowedForNewUsers = true;
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;


        });

        return Services;
    }
    public static IServiceCollection AddCORS(this IServiceCollection Services)
    {
        Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder
                        //.WithMethods("GET", "POST", "PUT", "DELETE")
                        //.WithOrigins("http://localhost:3000")
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
        });
        return Services;
    }
}
//public static IServiceCollection AddHangfire(this IServiceCollection Services, IConfiguration configuration)
//{
//    Services.AddHangfire(config => config
//    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
//    .UseSimpleAssemblyNameTypeSerializer()
//    .UseRecommendedSerializerSettings()
//    .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

//    // Add the processing server as IHostedService
//    Services.AddHangfireServer();
//    return Services;
//}


