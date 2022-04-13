using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Optional;
using Xunit;

namespace AutoMapper.Extensions.Optional.Tests
{
  public class OptionSourceMapperTests
  {
    private readonly IFixture _fixture = new Fixture();

    #region Helpers

    private object CreateOption(object o, Type type) => MakeStaticGeneric(nameof(Some), type).Invoke(null, new[] {o});

    private MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      GetType()
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");

    private static Option<T> Some<T>(T value) => value.Some();

    private static IMapper CreateMapper()
    {
      var mapperConfiguration = new MapperConfiguration(configuration =>
      {
        configuration.Mappers.Insert(0, new OptionSourceMapper());
      });


      return mapperConfiguration.CreateMapper();
    }

    private static Type CreateConcreteOptionType(Type concreteType) => typeof(Option<>).MakeGenericType(concreteType);

    #endregion Helpers
    
    [Theory]
    [ClassData(typeof(TypesData))]
    public void Can_map_from_Option(Type sourceType, Type destinationType)
    {
      // Arrange
      var sourceValue = _fixture.Create(sourceType, new SpecimenContext(_fixture));
      var optionSourceValue = CreateOption(sourceValue, sourceType);
      var optionSourceType = CreateConcreteOptionType(sourceType);

      var sut = CreateMapper();
      var mappedSourceValue = sut.Map(sourceValue, sourceType, destinationType);

      // Act
      Action act = () => sut.Map(optionSourceValue, optionSourceType, destinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(optionSourceValue, optionSourceType, destinationType);
      resultingValue.Should().BeEquivalentTo(mappedSourceValue);
    }
  }
}