namespace SurvayBasket.Application.Abstraction.Errors;

public static class VotesErrors
{
    public static readonly Error InvalidCredentials = new("Invalid credentials", "Invalid Poll Credentials", StatusCodes.Status404NotFound);
    public static readonly Error NotFound = new("Invalid credentials", "NotFound", StatusCodes.Status404NotFound);
    public static readonly Error DaplicatedVote = new("Vote.duplicatedVotes", "This user is already Voted for this Poll", StatusCodes.Status409Conflict);

}
