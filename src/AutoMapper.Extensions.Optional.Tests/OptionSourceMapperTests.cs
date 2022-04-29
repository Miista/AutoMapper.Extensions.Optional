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
    
    [Theory]
    // A -> A
    [InlineData(typeof(bool), default(bool), typeof(bool), default(bool))]
    [InlineData(typeof(char), default(char), typeof(char), default(char))]
    [InlineData(typeof(double), default(double), typeof(double), default(double))]
    [InlineData(typeof(float), default(float), typeof(float), default(float))]
    [InlineData(typeof(int), default(int), typeof(int), default(int))]
    [InlineData(typeof(long), default(long), typeof(long), default(long))]
    [InlineData(typeof(short), default(short), typeof(short), default(short))]
    [InlineData(typeof(string), null, typeof(string), null)]
    [InlineData(typeof(uint), default(uint), typeof(uint), default(uint))]
    [InlineData(typeof(ulong), default(ulong), typeof(ulong), default(ulong))]
    [InlineData(typeof(ushort), default(ushort), typeof(ushort), default(ushort))]
    // A? -> A?
    [InlineData(typeof(bool?), null, typeof(bool?), null)]
    [InlineData(typeof(char?), null, typeof(char?), null)]
    [InlineData(typeof(double?), null, typeof(double?), null)]
    [InlineData(typeof(float?), null, typeof(float?), null)]
    [InlineData(typeof(int?), null, typeof(int?), null)]
    [InlineData(typeof(long?), null, typeof(long?), null)]
    [InlineData(typeof(short?), null, typeof(short?), null)]
    [InlineData(typeof(uint?), null, typeof(uint?), null)]
    [InlineData(typeof(ulong?), null, typeof(ulong?), null)]
    [InlineData(typeof(ushort?), null, typeof(ushort?), null)]
    [InlineData(typeof(Uri), null, typeof(Uri), null)]
    // A -> B
    [InlineData(typeof(bool), true, typeof(int), 1)]
    [InlineData(typeof(bool), false, typeof(int), 0)]
    [InlineData(typeof(int), 1, typeof(double), 1.0)]
    public void Can_map_from_Option1(Type sourceType, object sourceValue, Type destinationType, object expectedDestinationValue)
    {
      // Arrange
      var source = CreateOption(sourceValue, sourceType);
      var optionSourceType = CreateConcreteOptionType(sourceType);

      var sut = CreateMapper();

      // Act
      Action act = () => sut.Map(source, optionSourceType, destinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(source, optionSourceType, destinationType);
      resultingValue.Should().BeEquivalentTo(expectedDestinationValue);
    }
    
    [Theory]
    [MemberData(nameof(Maps_A_to_A_Data))]
    public void Maps_Option_A_to_A(
      Type sourceType,
      object sourceValue,
      Type destinationType,
      object expectedDestinationValue)
    {
      // Arrange
      var source = sourceValue;
      var optionSourceType = CreateConcreteOptionType(sourceType);

      var sut = CreateMapper();

      // Act
      Action act = () => sut.Map(source, optionSourceType, destinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(source, optionSourceType, destinationType);
      resultingValue.Should().BeEquivalentTo(expectedDestinationValue);
    }

    public static IEnumerable<object[]> Maps_A_to_A_Data
    {
      get
      {
        // A -> A
        yield return new object[] {typeof(bool), default(bool).Some(), typeof(bool), default(bool)};
        yield return new object[] {typeof(char), default(char).Some(), typeof(char), default(char)};
        yield return new object[] {typeof(double), default(double).Some(), typeof(double), default(double)};
        yield return new object[] {typeof(float), default(float).Some(), typeof(float), default(float)};
        yield return new object[] {typeof(int), default(int).Some(), typeof(int), default(int)};
        yield return new object[] {typeof(long), default(long).Some(), typeof(long), default(long)};
        yield return new object[] {typeof(short), default(short).Some(), typeof(short), default(short)};
        yield return new object[] {typeof(string), "Hello, World!".Some(), typeof(string), "Hello, World!"};
        yield return new object[] {typeof(uint), default(uint).Some(), typeof(uint), default(uint)};
        yield return new object[] {typeof(ulong), default(ulong).Some(), typeof(ulong), default(ulong)};
        yield return new object[] {typeof(ushort), default(ushort).Some(), typeof(ushort), default(ushort)};
        yield return new object[] {typeof(bool), default(bool).Some(), typeof(bool), default(bool)};
        yield return new object[] {typeof(bool), default(bool).Some(), typeof(bool), default(bool)};
        
        // A? -> A?
        yield return new object[] {typeof(bool?), Option.None<bool?>(), typeof(bool?), default(bool?)};
        yield return new object[] {typeof(char?), Option.None<char?>(), typeof(char?), default(char?)};
        yield return new object[] {typeof(double?), Option.None<double?>(), typeof(double?), default(double?)};
        yield return new object[] {typeof(float?), Option.None<float?>(), typeof(float?), default(float?)};
        yield return new object[] {typeof(int?), Option.None<int?>(), typeof(int?), default(int?)};
        yield return new object[] {typeof(long?), Option.None<long?>(), typeof(long?), default(long?)};
        yield return new object[] {typeof(short?), Option.None<short?>(), typeof(short?), default(short?)};
        yield return new object[] {typeof(uint?), Option.None<uint?>(), typeof(uint?), default(uint?)};
        yield return new object[] {typeof(ulong?), Option.None<ulong?>(), typeof(ulong?), default(ulong?)};
        yield return new object[] {typeof(ushort?), Option.None<ushort?>(), typeof(ushort?), default(ushort?)};
        yield return new object[] {typeof(Uri), Option.None<Uri>(), typeof(Uri), default(Uri)};
        
        // A? -> A
        yield return new object[] {typeof(bool?), Option.None<bool?>(), typeof(bool), default(bool)};
        yield return new object[] {typeof(char?), Option.None<char?>(), typeof(char), default(char)};
        yield return new object[] {typeof(double?), Option.None<double?>(), typeof(double), default(double)};
        yield return new object[] {typeof(float?), Option.None<float?>(), typeof(float), default(float)};
        yield return new object[] {typeof(int?), Option.None<int?>(), typeof(int), default(int)};
        yield return new object[] {typeof(long?), Option.None<long?>(), typeof(long), default(long)};
        yield return new object[] {typeof(short?), Option.None<short?>(), typeof(short), default(short)};
        yield return new object[] {typeof(uint?), Option.None<uint?>(), typeof(uint), default(uint)};
        yield return new object[] {typeof(ulong?), Option.None<ulong?>(), typeof(ulong), default(ulong)};
        yield return new object[] {typeof(ushort?), Option.None<ushort?>(), typeof(ushort), default(ushort)};
        
        // A -> B
        yield return new object[] {typeof(bool), true.Some(), typeof(int), 1};
        yield return new object[] {typeof(bool), false.Some(), typeof(int), 0};
        yield return new object[] {typeof(int), 1.Some(), typeof(double), 1.0};
      }
    }  

    
    [Theory]
    [InlineData(typeof(bool), default(bool))]
    [InlineData(typeof(char), default(char))]
    [InlineData(typeof(double), default(double))]
    [InlineData(typeof(float), default(float))]
    [InlineData(typeof(int), default(int))]
    [InlineData(typeof(long), default(long))]
    [InlineData(typeof(short), default(short))]
    [InlineData(typeof(string), null)]
    [InlineData(typeof(uint), default(uint))]
    [InlineData(typeof(ulong), default(ulong))]
    [InlineData(typeof(ushort), default(ushort))]
    [InlineData(typeof(bool?), null)]
    [InlineData(typeof(char?), null)]
    [InlineData(typeof(double?), null)]
    [InlineData(typeof(float?), null)]
    [InlineData(typeof(int?), null)]
    [InlineData(typeof(long?), null)]
    [InlineData(typeof(short?), null)]
    [InlineData(typeof(uint?), null)]
    [InlineData(typeof(ulong?), null)]
    [InlineData(typeof(ushort?), null)]
    [InlineData(typeof(Uri), null)]
    public void Can_map_from_None(Type destinationType, object expectedValue)
    {
      // Arrange
      var sourceValue = CreateNone(destinationType);
      var optionSourceType = CreateConcreteOptionType(destinationType);

      var sut = CreateMapper();

      // Act
      Action act = () => sut.Map(sourceValue, optionSourceType, destinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(sourceValue, optionSourceType, destinationType);
      resultingValue.Should().BeEquivalentTo(expectedValue);
    }
    
    private object CreateNone(Type type) => MakeStaticGeneric(nameof(None), type).Invoke(null, new object[0]);
    
    private static Option<T> None<T>() => Option.None<T>();
  }
}