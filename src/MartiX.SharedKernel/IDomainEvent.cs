using Mediator;

namespace MartiX.SharedKernel;

/// <summary>
/// Represents a domain event raised by an aggregate.
/// </summary>
public interface IDomainEvent : INotification
{
  /// <summary>
  /// Gets the UTC timestamp when the event occurred.
  /// </summary>
  /// <returns>The event occurrence time.</returns>
  DateTime DateOccurred { get; }
}
