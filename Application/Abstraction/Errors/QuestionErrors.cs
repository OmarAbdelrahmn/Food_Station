namespace SurvayBasket.Application.Abstraction.Errors;

public static class QuestionErrors
{
    public static readonly Error InvalidCredentials = new("Invalid credentials", "Invalid Question Credentials", StatusCodes.Status404NotFound);
    public static readonly Error InvalidQuestions = new("Invalid Question", "Invalid Question to Select", StatusCodes.Status409Conflict);
    public static readonly Error DaplicatedTitle = new("Invalid credentials", "Daplicated Question info", StatusCodes.Status409Conflict);
    public static readonly Error NotFound = new("Invalid credentials", "Question not found", StatusCodes.Status404NotFound);

}
