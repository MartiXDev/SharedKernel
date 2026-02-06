namespace MartiX.SharedKernel.UnitTests.ValueObjectTests;
/// <summary>
/// Tests value object hash code behavior.
/// </summary>
public class ValueObject_HashEquality
{
  /// <summary>
  /// Verifies that value objects with the same values have the same hash code.
  /// </summary>
  [Fact]
  public void WithSameValuesHaveSameHashCode()
  {
    // Arrange
    var valueObject1 = new TestValueObject(1);
    var valueObject2 = new TestValueObject(1);

    // Act & Assert
    valueObject1.GetHashCode().Should().Be(valueObject2.GetHashCode());
  }

  /// <summary>
  /// Verifies that value objects with different values are not equal.
  /// </summary>
  [Fact]
  public void WithDifferentValuesAreNotEqual()
  {
    // Arrange
    var valueObject1 = new TestValueObject(1);
    var valueObject2 = new TestValueObject(2);

    // Act & Assert
    valueObject1.Should().NotBe(valueObject2);
  }
}
