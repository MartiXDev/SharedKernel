using Mediator;

namespace MartiX.SharedKernel;

public interface IDomainEvent : INotification
{
  DateTime DateOccurred { get; }
}
