using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Optional;
using Xunit;

namespace AutoMapper.Extensions.Optional.Tests
{
  public class OptionDestinationMapperTests
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
        configuration.Mappers.Insert(0, new OptionDestinationMapper());
      });


      return mapperConfiguration.CreateMapper();
    }
    
    private static Type CreateConcreteOptionType(Type concreteType) => typeof(Option<>).MakeGenericType(concreteType);
    
    #endregion Helpers
    
    [Theory]
    [ClassData(typeof(TypesData))]
    public void Can_map_to_Option(Type sourceType, Type destinationType)
    {
      // Arrange
      var sourceValue = _fixture.Create(sourceType, new SpecimenContext(_fixture));
      var optionDestinationType = CreateConcreteOptionType(destinationType);

      var sut = CreateMapper();
      var mappedSourceValue = sut.Map(sourceValue, sourceType, destinationType);
      var mappedOption = CreateOption(mappedSourceValue, destinationType);

      // Act
      Action act = () => sut.Map(sourceValue, sourceType, optionDestinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(sourceValue, sourceType, optionDestinationType);
      resultingValue.Should().BeEquivalentTo(mappedOption);
    }
  }
}