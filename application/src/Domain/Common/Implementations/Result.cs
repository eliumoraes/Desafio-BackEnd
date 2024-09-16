using Domain.Common.Interfaces;

namespace Domain.Common.Implementations;

public class Result<T> : IResult<T>
{
    public bool IsSuccess { get; private set; }
    public IEnumerable<string> Errors { get; }
    public T? Entity { get; }

    protected Result(bool isSuccess, List<string> errors, T entity)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        Entity = entity;
    }

    public static Result<T> Success(T entity)
    {
        return new Result<T>(true, new List<string>(), entity);
    }

    public static Result<T> Fail(List<string> errors, T? entity = default)
    {
        return new Result<T>(false, errors, entity);
    }
}