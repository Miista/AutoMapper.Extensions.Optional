using System;
using System.Collections.Generic;
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

    private static Type CreateConcreteOptionType(Type concreteType) => typeof(Option<>).MakeGenericType(concreteType);
    
    [Theory]
    [MemberData(nameof(Maps_A_to_A_Data))]
    public void Maps_A_to_A(
      Type sourceType,
      object sourceValue,
      Type destinationType,
      object expectedDestinationValue)
    {
      // Arrange
      var source = sourceValue;
      var optionSourceType = CreateConcreteOptionType(sourceType);
      var optionDestinationType = CreateConcreteOptionType(destinationType);
      var sut = CreateMapper();

      // Act
      Action act = () => sut.Map(source, optionSourceType, optionDestinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(source, optionSourceType, optionDestinationType);
      resultingValue.Should().BeEquivalentTo(expectedDestinationValue);
    }
    
    public static IEnumerable<object[]> Maps_A_to_A_Data
    {
      get
      {
        // A -> A
        yield return new object[] {typeof(bool), default(bool).Some(), typeof(bool), default(bool).Some()};
        yield return new object[] {typeof(char), default(char).Some(), typeof(char), default(char).Some()};
        yield return new object[] {typeof(double), default(double).Some(), typeof(double), default(double).Some()};
        yield return new object[] {typeof(float), default(float).Some(), typeof(float), default(float).Some()};
        yield return new object[] {typeof(int), default(int).Some(), typeof(int), default(int).Some()};
        yield return new object[] {typeof(long), default(long).Some(), typeof(long), default(long).Some()};
        yield return new object[] {typeof(short), default(short).Some(), typeof(short), default(short).Some()};
        yield return new object[] {typeof(string), "Hello, World!".Some(), typeof(string), "Hello, World!".Some()};
        yield return new object[] {typeof(uint), default(uint).Some(), typeof(uint), default(uint).Some()};
        yield return new object[] {typeof(ulong), default(ulong).Some(), typeof(ulong), default(ulong).Some()};
        yield return new object[] {typeof(ushort), default(ushort).Some(), typeof(ushort), default(ushort).Some()};
        yield return new object[] {typeof(bool), default(bool).Some(), typeof(bool), default(bool).Some()};
        yield return new object[] {typeof(bool), default(bool).Some(), typeof(bool), default(bool).Some()};
        
        // A? -> A?
        yield return new object[] {typeof(bool?), Option.None<bool?>(), typeof(bool?), Option.None<bool?>()};
        yield return new object[] {typeof(char?), Option.None<char?>(), typeof(char?), Option.None<char?>()};
        yield return new object[] {typeof(double?), Option.None<double?>(), typeof(double?), Option.None<double?>()};
        yield return new object[] {typeof(float?), Option.None<float?>(), typeof(float?), Option.None<float?>()};
        yield return new object[] {typeof(int?), Option.None<int?>(), typeof(int?), Option.None<int?>()};
        yield return new object[] {typeof(long?), Option.None<long?>(), typeof(long?), Option.None<long?>()};
        yield return new object[] {typeof(short?), Option.None<short?>(), typeof(short?), Option.None<short?>()};
        yield return new object[] {typeof(uint?), Option.None<uint?>(), typeof(uint?), Option.None<uint?>()};
        yield return new object[] {typeof(ulong?), Option.None<ulong?>(), typeof(ulong?), Option.None<ulong?>()};
        yield return new object[] {typeof(ushort?), Option.None<ushort?>(), typeof(ushort?), Option.None<ushort?>()};
        yield return new object[] {typeof(Uri), Option.None<Uri>(), typeof(Uri), Option.None<Uri>()};
        
        // A? -> A
        yield return new object[] {typeof(bool?), Option.None<bool?>(), typeof(bool), Option.None<bool>()};
        yield return new object[] {typeof(char?), Option.None<char?>(), typeof(char), Option.None<char>()};
        yield return new object[] {typeof(double?), Option.None<double?>(), typeof(double), Option.None<double>()};
        yield return new object[] {typeof(float?), Option.None<float?>(), typeof(float), Option.None<float>()};
        yield return new object[] {typeof(int?), Option.None<int?>(), typeof(int), Option.None<int>()};
        yield return new object[] {typeof(long?), Option.None<long?>(), typeof(long), Option.None<long>()};
        yield return new object[] {typeof(short?), Option.None<short?>(), typeof(short), Option.None<short>()};
        yield return new object[] {typeof(uint?), Option.None<uint?>(), typeof(uint), Option.None<uint>()};
        yield return new object[] {typeof(ulong?), Option.None<ulong?>(), typeof(ulong), Option.None<ulong>()};
        yield return new object[] {typeof(ushort?), Option.None<ushort?>(), typeof(ushort), Option.None<ushort>()};
        
        // A -> B
        yield return new object[] {typeof(bool), true.Some(), typeof(int), 1.Some()};
        yield return new object[] {typeof(bool), false.Some(), typeof(int), 0.Some()};
        yield return new object[] {typeof(int), 1.Some(), typeof(double), 1.0.Some()};
      }
    }  
    
    #region Helpers

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
    
    #endregion Helpers
  }
}