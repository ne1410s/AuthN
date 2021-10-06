namespace AuthN.Api.WebModels
{
    /// <summary>
    /// Response returned from a unsuccessful http call.
    /// </summary>
    /// <param name="Type">The error type.</param>
    /// <param name="Message">The error message.</param>
    /// <param name="Errors">Any associated error details.</param>
    public record HttpErrorResponse(
        string Type,
        string Message,
        string[]? Errors = null);
}
