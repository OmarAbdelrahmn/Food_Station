
namespace SurvayBasket.Application.Abstraction.Errors;

public static class PollsErrors
{
    public static readonly Error InvalidCredentials = new("Invalid credentials", "Invalid Poll Credentials", StatusCodes.Status404NotFound);
    public static readonly Error NotFound = new("Invalid credentials", "NotFound", StatusCodes.Status404NotFound);
    public static readonly Error DaplicatedTitle = new("Invalid credentials", "Daplicated Poll Title", StatusCodes.Status409Conflict);

}
