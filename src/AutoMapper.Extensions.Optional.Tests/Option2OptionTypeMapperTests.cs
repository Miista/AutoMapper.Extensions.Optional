using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Optional;
using Xunit;

namespace AutoMapper.Extensions.Optional.Tests
{
  public class Option2OptionTypeMapperTests
  {
    private readonly IFixture _fixture = new Fixture();

    [Theory]
    [ClassData(typeof(TypesData))]
    public void Can_map_between_Options(Type sourceType, Type destinationType)
    {
      // Arrange
      var optionSourceType = typeof(Option<>).MakeGenericType(sourceType);
      var optionDestinationType = typeof(Option<>).MakeGenericType(destinationType);
      var value = _fixture.Create(sourceType, new SpecimenContext(_fixture));
      var sourceValue = CreateOption(value, sourceType);

      var sut = CreateMapper();

      var mappedSourceValue = sut.Map(value, sourceType, destinationType);
      var mappedDestinationValue = CreateOption(mappedSourceValue, destinationType);

      // Act
      Action act = () => sut.Map(sourceValue, optionSourceType, optionDestinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(sourceValue, optionSourceType, optionDestinationType);
      resultingValue.Should().BeEquivalentTo(mappedDestinationValue);
    }
    
    private static IMapper CreateMapper()
    {
      var mapperConfiguration = new MapperConfiguration(configuration =>
      {
        configuration.CreateMap(typeof(Option<>), typeof(Option<>)).ConvertUsing(typeof(Option2OptionTypeMapper<,>));
      });


      return mapperConfiguration.CreateMapper();
    }

    private object CreateOption(object o, Type type) => MakeStaticGeneric(nameof(Some), type).Invoke(null, new[] {o});

    private MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      GetType()
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");

    private static Option<T> Some<T>(T value) => value.Some();
  }
}