using KVKarco.CommonTypes;

namespace KVKarco.SimpleResult;

public sealed class ValidationError : Error
{
    private readonly Dictionary<string, List<string>> _validationErrors;

    internal ValidationError(string code)
        : base(code, "One or more ValidationErrors occurred", ErrorTypes.Validation)
    {
        _validationErrors = [];
    }

    public bool HasErrors => _validationErrors.Count > 0;
    public IReadOnlyDictionary<string, IEnumerable<string>>? ValidationErrors => HasErrors ?
        _validationErrors.ToDictionary(key => key.Key, value => value.Value.AsEnumerable())
        : null;

    public void TryAddPropertyErrors(params PropertyError[] errors)
    {
        if (errors == null || errors.Length == 0)
        {
            return;
        }

        foreach (var error in errors)
        {
            if (error == default || error.ErrorMessages.Count == 0)
            {
                continue;
            }

            if (!_validationErrors.ContainsKey(error.PropertyName))
            {
                _validationErrors.Add(error.PropertyName, [.. error.ErrorMessages]);
            }
        }

        return;
    }

    public void TryAddPropertyErrors(string? propertyName, params string[] errorMessages)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || errorMessages == null || errorMessages.Length == 0)
        {
            return;
        }

        if (!_validationErrors.ContainsKey(propertyName))
        {
            _validationErrors.Add(propertyName, [.. errorMessages]);
        }
    }
}
