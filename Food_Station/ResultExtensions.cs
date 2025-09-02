﻿using Microsoft.AspNetCore.Mvc;
using SurvayBasket.Application.Abstraction;
using ObjectResult = Microsoft.AspNetCore.Mvc.ObjectResult;

namespace Application.Abstraction;

public static class ResultExtensions
{
    public static Microsoft.AspNetCore.Mvc.ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert a successful result to a problem.");

        // Create ProblemDetails directly since Result.Problem does not exist
        var problemDetails = new ProblemDetails
        {
            Status = result.Error.StatuesCode,
            Title = result.Error.Code,
            Detail = result.Error.Description
        };

        problemDetails.Extensions = new Dictionary<string, object?>
        {
            { "error", new { result.Error.Code , result.Error.Description } }
        };

        return new ObjectResult(problemDetails);
    }
}
