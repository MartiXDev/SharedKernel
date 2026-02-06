using Mediator;

namespace MartiX.SharedKernel.UnitTests.DomainEventBaseTests;

/// <summary>
/// Tests the default constructor behavior of <see cref="DomainEventBase"/>.
/// </summary>
public class DomainEventBase_Constructor : INotificationHandler<DomainEventBase_Constructor.TestDomainEvent>
{
  /// <summary>
  /// Test domain event.
  /// </summary>
  public class TestDomainEvent : DomainEventBase { }

  /// <summary>
  /// Verifies that <see cref="DomainEventBase.DateOccurred"/> is set on creation.
  /// </summary>
  [Fact]
  public void SetsDateOccurredToCurrentDateTime()
  {
    // Arrange
    var beforeCreation = DateTime.UtcNow;

    // Act
    var domainEvent = new TestDomainEvent();

    // Assert
    domainEvent.DateOccurred.Should().BeOnOrAfter(beforeCreation);
    domainEvent.DateOccurred.Should().BeOnOrBefore(DateTime.UtcNow);
  }

  /// <summary>
  /// Handles the test domain event.
  /// </summary>
  /// <param name="notification">The event to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A task representing the handler.</returns>
  public ValueTask Handle(TestDomainEvent notification, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
