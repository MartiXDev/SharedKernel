using Mediator;
using Microsoft.Extensions.Logging;

namespace MartiX.SharedKernel;

/// <summary>
/// Dispatches domain events using the Mediator pipeline.
/// </summary>
public class MediatorDomainEventDispatcher : IDomainEventDispatcher
{
  private readonly IMediator _mediator;
  private readonly ILogger<MediatorDomainEventDispatcher> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="MediatorDomainEventDispatcher"/> class.
  /// </summary>
  /// <param name="mediator">The mediator used to publish domain events.</param>
  /// <param name="logger">The logger instance.</param>
  public MediatorDomainEventDispatcher(IMediator mediator, ILogger<MediatorDomainEventDispatcher> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  /// <summary>
  /// Dispatches and clears domain events for the provided entities.
  /// </summary>
  /// <param name="entitiesWithEvents">Entities that currently have pending domain events.</param>
  /// <returns>A task that completes when all events are dispatched.</returns>
  public async Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents)
  {
    foreach (IHasDomainEvents entity in entitiesWithEvents)
    {
      if (entity is IHasDomainEvents hasDomainEvents)
      {
        IDomainEvent[] events = hasDomainEvents.DomainEvents.ToArray();
        hasDomainEvents.ClearDomainEvents();

        foreach (var domainEvent in events)
          await _mediator.Publish(domainEvent).ConfigureAwait(false);
      }
      else
      {
        _logger.LogError(
          "Entity of type {EntityType} does not inherit from {BaseType}. Unable to clear domain events.",
          entity.GetType().Name,
          nameof(IHasDomainEvents));
      }
    }
  }
}
