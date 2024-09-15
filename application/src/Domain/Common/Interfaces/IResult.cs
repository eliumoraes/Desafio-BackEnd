namespace Domain.Common.Interfaces
{
    public interface IResult
    {
        bool IsSuccess { get; }
        IEnumerable<string> Errors { get; }
    }
}