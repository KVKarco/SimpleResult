namespace KVKarco.SimpleResult;

public static class ResultExtensions
{
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> success,
        Func<Result, TOut> failure)
    {
        if (result == null || success == null || failure == null)
        {
            throw new InvalidOperationException("success and failure need to be valid methods.");
        }
        return result.IsSuccess ? success() : failure(result);
    }

    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> success,
        Func<Result<TIn>, TOut> failure)
    {
        if (result == null || success == null || failure == null)
        {
            throw new InvalidOperationException("success and failure need to be valid methods.");
        }
        return result.IsSuccess ? success(result.Value!) : failure(result);
    }
}
