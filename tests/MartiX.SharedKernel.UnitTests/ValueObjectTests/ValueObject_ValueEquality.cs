namespace MartiX.SharedKernel.UnitTests.ValueObjectTests;

/// <summary>
/// Tests value object equality behavior.
/// </summary>
public class ValueObject_ValueEquality
{

  /// <summary>
  /// Verifies that value objects with the same values are equal.
  /// </summary>
  [Fact]
  public void WithSameValuesAreEqual()
  {
    // Arrange
    var valueObject1 = new TestValueObject(1);
    var valueObject2 = new TestValueObject(1);

    // Act & Assert
    valueObject1.Should().Be(valueObject2);
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
