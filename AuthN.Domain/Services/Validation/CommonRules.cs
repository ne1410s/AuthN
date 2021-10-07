namespace AuthN.Domain.Services.Validation
{
    /// <summary>
    /// Exposes control over a set of common validation rules.
    /// </summary>
    public record CommonRules(
        int UsernameMinLength = 6,
        int EmailMinLength = 6,
        int PasswordMinLength = 8,
        int TokenMaxMinutes = 60);
}
