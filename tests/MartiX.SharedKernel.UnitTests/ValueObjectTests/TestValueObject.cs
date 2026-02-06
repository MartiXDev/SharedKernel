namespace MartiX.SharedKernel.UnitTests.ValueObjectTests;

/// <summary>
/// Simple value object used for equality tests.
/// </summary>
public class TestValueObject : ValueObject
  {
    /// <summary>
    /// Gets the wrapped value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestValueObject"/> class.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public TestValueObject(int value)
    {
      Value = value;
    }

    /// <summary>
    /// Gets the components used for equality.
    /// </summary>
    /// <returns>The sequence of equality components.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Value;
    }
  }
