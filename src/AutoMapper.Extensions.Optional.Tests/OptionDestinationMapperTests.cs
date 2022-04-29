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
  public class OptionDestinationMapperTests
  {
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void Maps_null_to_None_for_reference_types()
    {
      // Arrange
      var sut = CreateMapper();
      
      // Act
      Func<Option<string>> act = () => sut.Map<Option<string>>(null);
      
      // Assert
      act.Should().NotThrow();

      act().Should().Be(Option.None<string>());
    }
    
    [Fact]
    public void Maps_null_to_None_for_nullable_types_which_are_null()
    {
      // Arrange
      var sut = CreateMapper();
      
      // Act
      Func<Option<double>> act = () => sut.Map<Option<double>>((double?)null);
      
      // Assert
      act.Should().NotThrow();

      act().Should().Be(Option.None<double>());
    }
    
    /* We need to support the following:
        1. Mapping null to None for
          a. Reference types
          b. Nullable values types which are null
        2. Mapping A -> A
        3. Mapping A -> B
     */
    
    #region Helpers
    
    private object CreateOption(object o, Type type) => MakeStaticGeneric(nameof(Some), type).Invoke(null, new[] {o});

    private MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      GetType()
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");

    private static Option<T> Some<T>(T value) => value.SomeNotNull();

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
    [MemberData(nameof(Maps_A_to_Option_A_Data))]
    public void Maps_A_to_Option_A(
      Type sourceType,
      object sourceValue,
      Type destinationType,
      object expectedDestinationValue)
    {
      // Arrange
      var source = sourceValue;
      var optionDestinationType = CreateConcreteOptionType(destinationType);

      var sut = CreateMapper();

      // Act
      Action act = () => sut.Map(source, sourceType, optionDestinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(source, sourceType, optionDestinationType);
      resultingValue.Should().BeEquivalentTo(expectedDestinationValue);
    }

    public static IEnumerable<object[]> Maps_A_to_Option_A_Data
    {
      get
      {
        // A -> A
        yield return new object[] {typeof(bool), default(bool), typeof(bool), default(bool).Some()};
        yield return new object[] {typeof(char), default(char), typeof(char), default(char).Some()};
        yield return new object[] {typeof(double), default(double), typeof(double), default(double).Some()};
        yield return new object[] {typeof(float), default(float), typeof(float), default(float).Some()};
        yield return new object[] {typeof(int), default(int), typeof(int), default(int).Some()};
        yield return new object[] {typeof(long), default(long), typeof(long), default(long).Some()};
        yield return new object[] {typeof(short), default(short), typeof(short), default(short).Some()};
        yield return new object[] {typeof(string), "Hello, World!", typeof(string), "Hello, World!".Some()};
        yield return new object[] {typeof(uint), default(uint), typeof(uint), default(uint).Some()};
        yield return new object[] {typeof(ulong), default(ulong), typeof(ulong), default(ulong).Some()};
        yield return new object[] {typeof(ushort), default(ushort), typeof(ushort), default(ushort).Some()};
        yield return new object[] {typeof(bool), default(bool), typeof(bool), default(bool).Some()};
        yield return new object[] {typeof(bool), default(bool), typeof(bool), default(bool).Some()};
        
        // A? -> A?
        yield return new object[] {typeof(bool?), (bool?)null, typeof(bool), Option.None<bool>()};
        yield return new object[] {typeof(char?), null, typeof(char), Option.None<char>()};
        yield return new object[] {typeof(double?), null, typeof(double), Option.None<double>()};
        yield return new object[] {typeof(float?), null, typeof(float), Option.None<float>()};
        yield return new object[] {typeof(int?), null, typeof(int), Option.None<int>()};
        yield return new object[] {typeof(long?), null, typeof(long), Option.None<long>()};
        yield return new object[] {typeof(short?), null, typeof(short), Option.None<short>()};
        yield return new object[] {typeof(uint?), null, typeof(uint), Option.None<uint>()};
        yield return new object[] {typeof(ulong?), null, typeof(ulong), Option.None<ulong>()};
        yield return new object[] {typeof(ushort?), null, typeof(ushort), Option.None<ushort>()};
        yield return new object[] {typeof(Uri), null, typeof(Uri), Option.None<Uri>()};
        yield return new object[] {typeof(bool?), default(bool), typeof(bool), default(bool).Some()};
        yield return new object[] {typeof(char?), default(char), typeof(char), default(char).Some()};
        yield return new object[] {typeof(double?), default(double), typeof(double), default(double).Some()};
        yield return new object[] {typeof(float?), default(float), typeof(float), default(float).Some()};
        yield return new object[] {typeof(int?), default(int), typeof(int), default(int).Some()};
        yield return new object[] {typeof(long?), default(long), typeof(long), default(long).Some()};
        yield return new object[] {typeof(short?), default(short), typeof(short), default(short).Some()};
        yield return new object[] {typeof(uint?), default(uint), typeof(uint), default(uint).Some()};
        yield return new object[] {typeof(ulong?), default(ulong), typeof(ulong), default(ulong).Some()};
        yield return new object[] {typeof(ushort?), default(ushort), typeof(ushort), default(ushort).Some()};
        
        // A? -> A
        yield return new object[] {typeof(bool?), null, typeof(bool), Option.None<bool>()};
        yield return new object[] {typeof(char?), null, typeof(char), Option.None<char>()};
        yield return new object[] {typeof(double?), null, typeof(double), Option.None<double>()};
        yield return new object[] {typeof(float?), null, typeof(float), Option.None<float>()};
        yield return new object[] {typeof(int?), null, typeof(int), Option.None<int>()};
        yield return new object[] {typeof(long?), null, typeof(long), Option.None<long>()};
        yield return new object[] {typeof(short?), null, typeof(short), Option.None<short>()};
        yield return new object[] {typeof(uint?), null, typeof(uint), Option.None<uint>()};
        yield return new object[] {typeof(ulong?), null, typeof(ulong), Option.None<ulong>()};
        yield return new object[] {typeof(ushort?), null, typeof(ushort), Option.None<ushort>()};
        /*
         * yield return new object[] {typeof(bool?), null, typeof(bool), default(bool)};
        yield return new object[] {typeof(char?), null, typeof(char), default(char)};
        yield return new object[] {typeof(double?), null, typeof(double), default(double)};
        yield return new object[] {typeof(float?), null, typeof(float), default(float)};
        yield return new object[] {typeof(int?), null, typeof(int), default(int)};
        yield return new object[] {typeof(long?), null, typeof(long), default(long)};
        yield return new object[] {typeof(short?), null, typeof(short), default(short)};
        yield return new object[] {typeof(uint?), null, typeof(uint), default(uint)};
        yield return new object[] {typeof(ulong?), null, typeof(ulong), default(ulong)};
        yield return new object[] {typeof(ushort?), null, typeof(ushort), default(ushort)};
         */
        
        // A -> B
        yield return new object[] {typeof(bool), true, typeof(int), 1.Some()};
        yield return new object[] {typeof(bool), false, typeof(int), 0.Some()};
        yield return new object[] {typeof(int), 1, typeof(double), 1.0.Some()};
      }
    }  
    
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
    
    [Theory]
    [ClassData(typeof(NullableTypesData))]
    public void Can_map_null_to_Option(Type sourceType)
    {
      // Arrange
      object sourceValue = null;
      var optionDestinationType = CreateConcreteOptionType(sourceType);

      var sut = CreateMapper();
      var mappedSourceValue = sut.Map(sourceValue, sourceType, sourceType);
      var mappedOption = CreateOption(mappedSourceValue, sourceType);

      // Act
      Action act = () => sut.Map(sourceValue, sourceType, optionDestinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = sut.Map(sourceValue, sourceType, optionDestinationType);
      resultingValue.Should().BeEquivalentTo(mappedOption);
    }
  }
}