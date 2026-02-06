namespace MartiX.SharedKernel;

/// <summary>
/// A base type for domain events. Depends on Mediator INotification.
/// Includes DateOccurred which is set on creation.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
  /// <summary>
  /// Gets the UTC timestamp when the event occurred.
  /// </summary>
  /// <returns>The event occurrence time.</returns>
  public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
