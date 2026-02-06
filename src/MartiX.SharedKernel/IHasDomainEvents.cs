namespace MartiX.SharedKernel;

/// <summary>
/// Exposes domain events raised by an entity or aggregate.
/// </summary>
public interface IHasDomainEvents
{
  /// <summary>
  /// Gets the pending domain events.
  /// </summary>
  /// <returns>The read-only collection of pending domain events.</returns>
  IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

  /// <summary>
  /// Clears all pending domain events.
  /// </summary>
  void ClearDomainEvents();
}
