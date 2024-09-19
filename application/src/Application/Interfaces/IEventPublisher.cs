using Domain.Common.Interfaces;

namespace Application.Interfaces;

public interface IEventPublisher
{
    Task<IResult<bool>> PublishAsync<T>(T @event) where T : class;
}