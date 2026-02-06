using MartiX.Specification;

namespace MartiX.SharedKernel;

/// <summary>
/// An abstraction for persistence, based on MartiX.Specification
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}
