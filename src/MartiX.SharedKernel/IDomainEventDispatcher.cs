namespace MartiX.SharedKernel;

/// <summary>
/// A simple interface for sending domain events. Can use MediatR or any other implementation.
/// </summary>
public interface IDomainEventDispatcher
{
  /// <summary>
  /// Dispatches and clears domain events for the provided entities.
  /// </summary>
  /// <param name="entitiesWithEvents">Entities that currently have pending domain events.</param>
  /// <returns>A task that completes when all events are dispatched.</returns>
  Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents);
}
