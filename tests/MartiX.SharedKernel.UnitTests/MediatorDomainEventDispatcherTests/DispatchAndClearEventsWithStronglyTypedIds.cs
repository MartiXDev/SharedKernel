using Mediator;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MartiX.SharedKernel.UnitTests.MediatorDomainEventDispatcherTests;

/// <summary>
/// Tests dispatching and clearing domain events with strongly typed identifiers.
/// </summary>
public class DispatchAndClearEventsWithStronglyTypedIds : IDomainEventHandler<DispatchAndClearEventsWithStronglyTypedIds.TestDomainEvent>
{
  /// <summary>
  /// Test domain event.
  /// </summary>
  public class TestDomainEvent : DomainEventBase;

  /// <summary>
  /// Test strongly typed identifier.
  /// </summary>
  public readonly record struct StronglyTyped;

  /// <summary>
  /// Test entity with a strongly typed identifier.
  /// </summary>
  private sealed class TestEntityStronglyTyped : EntityBase<StronglyTyped>
  {
    /// <summary>
    /// Adds a test domain event.
    /// </summary>
    public void AddTestDomainEvent()
    {
      TestDomainEvent domainEvent = new();
      RegisterDomainEvent(domainEvent);
    }
  }

  /// <summary>
  /// Verifies that domain events are dispatched and cleared.
  /// </summary>
  [Fact]
  public async Task CallsPublishAndClearDomainEventsWithStronglyTypedId()
  {
    // Arrange
    Mock<IMediator> mediatorMock = new Mock<IMediator>();
    MediatorDomainEventDispatcher domainEventDispatcher =
      new(mediatorMock.Object, NullLogger<MediatorDomainEventDispatcher>.Instance);
    TestEntityStronglyTyped entity = new();
    entity.AddTestDomainEvent();

    // Act
    await domainEventDispatcher.DispatchAndClearEvents(new List<IHasDomainEvents> { entity });

    // Assert
    mediatorMock.Verify(m => m.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    entity.DomainEvents.Should().BeEmpty();
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
