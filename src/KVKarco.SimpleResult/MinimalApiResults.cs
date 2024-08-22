using Microsoft.AspNetCore.Http;

namespace KVKarco.SimpleResult;

/// <summary>
/// Result helper class for mapping result objects to Minimal endpoints results.
/// </summary>
public static class MinimalApiResults
{
    /// <summary>
    /// Map failed Result into specific ProblemDetails base of the Error info.
    /// </summary>
    public static IResult Problem(Result result)
    {
        ArgumentNullException.ThrowIfNull(result, nameof(result));

        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cant create problemDetails from successful result.");
        }

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.ErrorType),
            statusCode: GetStatusCode(result.Error.ErrorType),
            extensions: GetErrors(result)
            );
    }

    private static string GetTitle(Error error) => error.ErrorType switch
    {
        ErrorTypes.NotFound or
        ErrorTypes.Validation or
        ErrorTypes.Conflict or
        ErrorTypes.Problem => error.Code,
        _ => "Server failure."
    };
    private static string GetDetail(Error error) => error.ErrorType switch
    {
        ErrorTypes.NotFound or
        ErrorTypes.Validation or
        ErrorTypes.Conflict or
        ErrorTypes.Problem => error.Description,
        _ => "An unexpected error occurred."
    };
    private static string GetType(ErrorTypes errorType) => errorType switch
    {
        ErrorTypes.Validation => "https://datatracker.ietf.org/doc/html/rfc9110#name-422-unprocessable-content",
        ErrorTypes.Problem => "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
        ErrorTypes.NotFound => "https://datatracker.ietf.org/doc/html/rfc9110#name-404-not-found",
        ErrorTypes.Conflict => "https://datatracker.ietf.org/doc/html/rfc9110#name-409-conflict",
        _ => "https://datatracker.ietf.org/doc/html/rfc9110#name-500-internal-server-error"
    };
    private static int GetStatusCode(ErrorTypes errorType) => errorType switch
    {
        ErrorTypes.Validation => StatusCodes.Status422UnprocessableEntity,
        ErrorTypes.Problem => StatusCodes.Status400BadRequest,
        ErrorTypes.NotFound => StatusCodes.Status404NotFound,
        ErrorTypes.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
    private static Dictionary<string, object?>? GetErrors(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?> { { "errors", validationError.ValidationErrors } };
    }
}
