namespace KVKarco.SimpleResult;

#pragma warning disable CA1716 // Identifiers should not match keywords
public class Error
#pragma warning restore CA1716 // Identifiers should not match keywords
{
    public readonly static Error None = new("None", "None", ErrorTypes.None);
    public readonly static Error NullValue = new("Error.NullValue", "Null value was provided.", ErrorTypes.None);

    protected internal Error(string code, string description, ErrorTypes errorType)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(description))
        {
            throw new InvalidOperationException("What is the point of creating not explanatory error. ");
        }
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorTypes ErrorType { get; }

    public static Error Problem(string code, string description) => new(code, description, ErrorTypes.Problem);
    public static Error NotFound(string code, string description) => new(code, description, ErrorTypes.NotFound);
    public static Error Conflict(string code, string description) => new(code, description, ErrorTypes.Conflict);
    public static ValidationError Validation(string code) => new(code);
}
