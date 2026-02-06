using Mediator;

namespace MartiX.SharedKernel.UnitTests.EntityBaseTests;

/// <summary>
/// Tests adding domain events to entities.
/// </summary>
public class EntityBase_AddDomainEvent : INotificationHandler<EntityBase_AddDomainEvent.TestDomainEvent>
{
  /// <summary>
  /// Test domain event.
  /// </summary>
  public class TestDomainEvent : DomainEventBase { }

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
  /// Verifies that a domain event is added to the entity.
  /// </summary>
  [Fact]
  public void AddsDomainEventToEntity()
  {
    // Arrange
    var entity = new TestEntity();

    // Act
    entity.AddTestDomainEvent();

    // Assert
    entity.DomainEvents.Should().HaveCount(1);
    entity.DomainEvents.Should().AllBeOfType<TestDomainEvent>();
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
