namespace MartiX.SharedKernel;

/// <summary>
/// NOTE: Use `readonly record struct` for most cases in C# 10+
/// See: https://nietras.com/2021/06/14/csharp-10-record-struct/
/// 
/// Alternately consider using Vogen
/// 
/// For this class implementation, reference:
/// See: https://enterprisecraftsmanship.com/posts/value-object-better-implementation/
/// </summary>
[Serializable]
public abstract class ValueObject : IComparable, IComparable<ValueObject>
{

  /// <summary>
  /// Returns the atomic values that participate in equality and ordering.
  /// </summary>
  /// <remarks>
  /// The order of returned components must be consistent and stable.
  /// </remarks>
  /// <returns>The ordered sequence of equality components.</returns>
  protected abstract IEnumerable<object?> GetEqualityComponents();

  /// <summary>
  /// Determines whether this value object is equal to another.
  /// </summary>
  /// <param name="obj">The object to compare against.</param>
  /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (GetUnproxiedType(this) != GetUnproxiedType(obj))
      return false;

    var valueObject = (ValueObject)obj;

    return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
  }

  /// <summary>
  /// Computes a hash code from the equality components.
  /// </summary>
  /// <returns>The hash code for this instance.</returns>
  public override int GetHashCode() =>
    GetEqualityComponents()
      .Aggregate(1, (current, obj) =>
      {
        unchecked
        {
          return current * 23 + (obj?.GetHashCode() ?? 0);
        }
      });

  /// <summary>
  /// Compares this value object to another for ordering.
  /// </summary>
  /// <param name="obj">The object to compare against.</param>
  /// <returns>A value indicating the relative order of the objects.</returns>
  public int CompareTo(object? obj)
  {
    if (obj == null)
      return 1;

    var thisType = GetUnproxiedType(this);
    var otherType = GetUnproxiedType(obj);

    if (thisType != otherType)
      return string.Compare(thisType.ToString(), otherType.ToString(), StringComparison.Ordinal);

    var other = (ValueObject)obj;

    var components = GetEqualityComponents().ToArray();
    var otherComponents = other.GetEqualityComponents().ToArray();
    var length = Math.Min(components.Length, otherComponents.Length);

    for (var i = 0; i < length; i++)
    {
      var comparison = CompareComponents(components[i], otherComponents[i]);
      if (comparison != 0)
        return comparison;
    }

    return components.Length.CompareTo(otherComponents.Length);
  }

  /// <summary>
  /// Compares this value object to another for ordering.
  /// </summary>
  /// <param name="other">The value object to compare against.</param>
  /// <returns>A value indicating the relative order of the objects.</returns>
  public int CompareTo(ValueObject? other)
  {
    return CompareTo(other as object);
  }

  private static int CompareComponents(object? object1, object? object2)
  {
    if (object1 is null && object2 is null)
      return 0;

    if (object1 is null)
      return -1;

    if (object2 is null)
      return 1;

    if (object1 is IComparable comparable1 && object2 is IComparable comparable2)
      return comparable1.CompareTo(comparable2);

    return object1.Equals(object2) ? 0 : -1;
  }

  /// <summary>
  /// Equality operator for value objects.
  /// </summary>
  /// <param name="a">The left operand.</param>
  /// <param name="b">The right operand.</param>
  /// <returns><c>true</c> if both operands are equal; otherwise, <c>false</c>.</returns>
  public static bool operator ==(ValueObject? a, ValueObject? b)
  {
    if (a is null && b is null)
      return true;

    if (a is null || b is null)
      return false;

    return a.Equals(b);
  }

  /// <summary>
  /// Inequality operator for value objects.
  /// </summary>
  /// <param name="a">The left operand.</param>
  /// <param name="b">The right operand.</param>
  /// <returns><c>true</c> if the operands are not equal; otherwise, <c>false</c>.</returns>
  public static bool operator !=(ValueObject? a, ValueObject? b)
  {
    return !(a == b);
  }

  /// <summary>
  /// Less-than operator for value objects.
  /// </summary>
  /// <param name="a">The left operand.</param>
  /// <param name="b">The right operand.</param>
  /// <returns><c>true</c> if <paramref name="a"/> is less than <paramref name="b"/>; otherwise, <c>false</c>.</returns>
  public static bool operator <(ValueObject? a, ValueObject? b)
  {
    if (a is null && b is null)
      return false;

    if (a is null)
      return true;

    return a.CompareTo(b) < 0;
  }

  /// <summary>
  /// Less-than-or-equal operator for value objects.
  /// </summary>
  /// <param name="a">The left operand.</param>
  /// <param name="b">The right operand.</param>
  /// <returns><c>true</c> if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
  public static bool operator <=(ValueObject? a, ValueObject? b)
  {
    return a < b || a == b;
  }

  /// <summary>
  /// Greater-than operator for value objects.
  /// </summary>
  /// <param name="a">The left operand.</param>
  /// <param name="b">The right operand.</param>
  /// <returns><c>true</c> if <paramref name="a"/> is greater than <paramref name="b"/>; otherwise, <c>false</c>.</returns>
  public static bool operator >(ValueObject? a, ValueObject? b)
  {
    if (a is null && b is null)
      return false;

    if (a is null)
      return false;

    if (b is null)
      return true;

    return a.CompareTo(b) > 0;
  }

  /// <summary>
  /// Greater-than-or-equal operator for value objects.
  /// </summary>
  /// <param name="a">The left operand.</param>
  /// <param name="b">The right operand.</param>
  /// <returns><c>true</c> if <paramref name="a"/> is greater than or equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
  public static bool operator >=(ValueObject? a, ValueObject? b)
  {
    return a > b || a == b;
  }

  /// <summary>
  /// Returns the underlying type, accounting for common ORM proxies.
  /// </summary>
  /// <param name="obj">The object whose underlying type is needed.</param>
  /// <returns>The non-proxy type.</returns>
  internal static Type GetUnproxiedType(object obj)
  {
    ArgumentNullException.ThrowIfNull(obj);

    const string EFCoreProxyPrefix = "Castle.Proxies.";
    const string NHibernateProxyPostfix = "Proxy";

    var type = obj.GetType();
    var typeString = type.ToString();

    if (typeString.Contains(EFCoreProxyPrefix) || typeString.EndsWith(NHibernateProxyPostfix))
      return type.BaseType!;

    return type;
  }
}
