namespace MartiX.SharedKernel;

public interface IHasDomainEvents
{
  IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
  void ClearDomainEvents();
}
