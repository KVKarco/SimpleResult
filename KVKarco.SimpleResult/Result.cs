namespace KVKarco.SimpleResult;

public class Result
{
    private readonly Error _error;
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Invalid creation of Result object.");
        }
        IsSuccess = isSuccess;
        _error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error => _error;

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static implicit operator Result(Error error) => Failure(error);
    public static implicit operator Result(bool isSuccess) => Success();

    public static Result FromError(Error error) => Failure(error);
    public static Result FromBoolean(bool isSuccess) => Success();
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => _value!;


    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

    public Result<TValue> FromTValue(TValue? value) =>
         value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    public new Result<TValue> FromError(Error error) => Failure<TValue>(error);
}
