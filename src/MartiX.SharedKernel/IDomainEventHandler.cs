using Mediator;

namespace MartiX.SharedKernel;

public interface IDomainEventHandler<T> : INotificationHandler<T> where T : IDomainEvent
{
}
