namespace MartiX.SharedKernel;

/// <summary>
/// A base class for DDD Entities. Includes support for domain events dispatched post-persistence.
/// If you prefer GUID Ids, change it here.
/// If you need to support both GUID and int IDs, change to EntityBase&lt;TId&gt; and use TId as the type for Id.
/// </summary>
public abstract class EntityBase : HasDomainEventsBase
{
  /// <summary>
  /// Gets or sets the entity identifier.
  /// </summary>
  public int Id { get; set; }
}

/// <summary>
/// Generic base class for entities with a custom identifier type.
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class EntityBase<TId> : HasDomainEventsBase
  where TId : struct, IEquatable<TId>
{
  /// <summary>
  /// Gets or sets the entity identifier.
  /// </summary>
  public TId Id { get; set; } = default!;
}

/// <summary>
/// For use with Vogen or similar tools for generating code for 
/// strongly typed Ids.
/// </summary>
/// <typeparam name="T">The derived entity type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class EntityBase<T, TId> : HasDomainEventsBase
  where T : EntityBase<T, TId>
{
  /// <summary>
  /// Gets or sets the entity identifier.
  /// </summary>
  public TId Id { get; set; } = default!;
}

