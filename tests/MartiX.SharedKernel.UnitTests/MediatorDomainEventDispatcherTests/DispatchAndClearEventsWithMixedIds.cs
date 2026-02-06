using Mediator;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MartiX.SharedKernel.UnitTests.MediatorDomainEventDispatcherTests;

/// <summary>
/// Tests dispatching and clearing domain events with mixed identifier types.
/// </summary>
public class DispatchAndClearEventsWithMixedIds : IDomainEventHandler<DispatchAndClearEventsWithMixedIds.TestDomainEvent>
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
  /// Test entity with int identifier.
  /// </summary>
  private sealed class TestEntity : EntityBase
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
  /// Test entity with Guid identifier.
  /// </summary>
  private sealed class TestEntityGuid : EntityBase<Guid>
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
  /// Test entity with strongly typed identifier.
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
  /// Verifies that domain events are dispatched and cleared for mixed identifier types.
  /// </summary>
  [Fact]
  public async Task CallsPublishAndClearDomainEventsWithStronglyTypedId()
  {
    // Arrange
    var mediatorMock = new Mock<IMediator>();
    var domainEventDispatcher = new MediatorDomainEventDispatcher(mediatorMock.Object, NullLogger<MediatorDomainEventDispatcher>.Instance);
    var entity = new TestEntity();
    var entityGuid = new TestEntityGuid();
    var entityStronglyTyped = new TestEntityStronglyTyped();
    entity.AddTestDomainEvent();
    entityGuid.AddTestDomainEvent();
    entityStronglyTyped.AddTestDomainEvent();

    // Act
    await domainEventDispatcher.DispatchAndClearEvents(new List<IHasDomainEvents> { entity, entityGuid, entityStronglyTyped });

    // Assert
    mediatorMock.Verify(m => m.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    entity.DomainEvents.Should().BeEmpty();
    entityGuid.DomainEvents.Should().BeEmpty();
    entityStronglyTyped.DomainEvents.Should().BeEmpty();
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
