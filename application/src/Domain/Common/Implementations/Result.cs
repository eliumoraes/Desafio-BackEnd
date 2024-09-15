using Domain.Common.Interfaces;

namespace Domain.Common.Implementations;

public class Result : IResult
{
    public bool IsSuccess { get; private set; }
    public IEnumerable<string> Errors { get; }

    protected Result(bool isSuccess, List<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success()
    {
        return new Result(true, new List<string>());
    }

    public static Result Fail(List<string> errors)
    {
        return new Result(false, errors);
    }
}