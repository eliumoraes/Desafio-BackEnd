namespace Domain.Common.Interfaces;
public interface IResult<T>
{
    bool IsSuccess { get; }
    IEnumerable<string> Errors { get; }
    T? Entity { get; }
}
