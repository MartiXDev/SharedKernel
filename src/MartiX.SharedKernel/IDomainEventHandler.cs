using Mediator;

namespace MartiX.SharedKernel;

/// <summary>
/// Handles a specific domain event type.
/// </summary>
/// <typeparam name="T">The domain event type.</typeparam>
public interface IDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent;
