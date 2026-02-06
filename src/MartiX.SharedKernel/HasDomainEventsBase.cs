using System.ComponentModel.DataAnnotations.Schema;

namespace MartiX.SharedKernel;

/// <summary>
/// Base class that stores and manages domain events for an entity.
/// </summary>
public abstract class HasDomainEventsBase : IHasDomainEvents
{
  private readonly List<IDomainEvent> _domainEvents = new();

  /// <summary>
  /// Gets the pending domain events.
  /// </summary>
  /// <returns>The read-only collection of pending domain events.</returns>
  [NotMapped]
  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  /// <summary>
  /// Registers a new domain event.
  /// </summary>
  /// <param name="domainEvent">The domain event to add.</param>
  protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);

  /// <summary>
  /// Clears all pending domain events.
  /// </summary>
  public void ClearDomainEvents() => _domainEvents.Clear();
}
