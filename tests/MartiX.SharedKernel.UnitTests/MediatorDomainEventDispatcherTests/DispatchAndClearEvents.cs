using Mediator;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MartiX.SharedKernel.UnitTests.MediatorDomainEventDispatcherTests;

/// <summary>
/// Tests dispatching and clearing domain events with int identifiers.
/// </summary>
public class DispatchAndClearEvents : IDomainEventHandler<DispatchAndClearEvents.TestDomainEvent>
{
  /// <summary>
  /// Test domain event.
  /// </summary>
  public class TestDomainEvent : DomainEventBase;

  /// <summary>
  /// Test entity that raises a domain event.
  /// </summary>
  private sealed class TestEntity : EntityBase
  {
    /// <summary>
    /// Adds a test domain event.
    /// </summary>
    public void AddTestDomainEvent()
    {
      var domainEvent = new TestDomainEvent();
      RegisterDomainEvent(domainEvent);
    }
  }

  /// <summary>
  /// Verifies that domain events are dispatched and cleared.
  /// </summary>
  [Fact]
  public async Task CallsPublishAndClearDomainEvents()
  {
    // Arrange
    var mediatorMock = new Mock<IMediator>();
    var domainEventDispatcher = new MediatorDomainEventDispatcher(mediatorMock.Object, NullLogger<MediatorDomainEventDispatcher>.Instance);
    var entity = new TestEntity();
    entity.AddTestDomainEvent();

    // Act
    await domainEventDispatcher.DispatchAndClearEvents(new List<EntityBase> { entity });

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
