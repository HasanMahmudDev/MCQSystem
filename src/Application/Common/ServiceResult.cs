namespace MCQSystem.Application.Common;

public sealed class ServiceResult<T>
{
    private ServiceResult(bool succeeded, T? value, string? error)
    {
        Succeeded = succeeded;
        Value = value;
        Error = error;
    }

    public bool Succeeded { get; }

    public T? Value { get; }

    public string? Error { get; }

    public static ServiceResult<T> Success(T value) => new(true, value, null);

    public static ServiceResult<T> Failure(string error) => new(false, default, error);
}
