namespace AutoCrudAdmin.ViewModels;

/// <summary>
/// Represents the result of a validation process in the AutoCrudAdmin application.
/// </summary>
public class ValidatorResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets a message about the validation result.
    /// </summary>
    public string Message { get; set; } = default!;

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A successful validation result.</returns>
    public static ValidatorResult Success()
        => new ()
        {
            IsValid = true,
        };

    /// <summary>
    /// Creates an unsuccessful validation result with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An unsuccessful validation result.</returns>
    public static ValidatorResult Error(string message)
        => new ()
        {
            IsValid = false,
            Message = message,
        };
}