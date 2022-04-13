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
    [MemberData(nameof(Can_map_to_Option_Data))]
    public void Can_map_between_Options(Type sourceType, Type destinationType)
    {
      // Arrange
      var optionSourceType = typeof(Option<>).MakeGenericType(sourceType);
      var optionDestinationType = typeof(Option<>).MakeGenericType(destinationType);
      var value = _fixture.Create(sourceType, new SpecimenContext(_fixture));
      var source = CreateOption(value, sourceType);
      var destination = CreateOption(value, destinationType);

      var mapperConfiguration = new MapperConfiguration(expression =>
      {
        expression.CreateMap(typeof(Option<>), typeof(Option<>)).ConvertUsing(typeof(Option2OptionTypeMapper<,>));
      });
      var mapper = mapperConfiguration.CreateMapper();

      // Act
      Action act = () => mapper.Map(source, destination, optionSourceType, optionDestinationType);

      // Assert
      act.Should().NotThrow();

      var resultingValue = mapper.Map(source, destination, optionSourceType, optionDestinationType);
      resultingValue.Should().BeEquivalentTo(destination);
    }

    private object CreateOption(object o, Type type) => MakeStaticGeneric(nameof(Some), type).Invoke(null, new[] {o});

    private MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      GetType()
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");

    private static Option<T> Some<T>(T value) => value.Some();

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public static IEnumerable<object[]> Can_map_to_Option_Data
    {
      get
      {
        // decimal is skipped because it causes an AmbiguousMatchException
        yield return new object[] {typeof(bool), typeof(bool)};
        yield return new object[] {typeof(string), typeof(string)};
        yield return new object[] {typeof(char), typeof(char)};
        yield return new object[] {typeof(byte), typeof(byte)};
        yield return new object[] {typeof(double), typeof(double)};
        yield return new object[] {typeof(double), typeof(string)};
        yield return new object[] {typeof(float), typeof(float)};
        yield return new object[] {typeof(short), typeof(short)};
        yield return new object[] {typeof(int), typeof(int)};
        yield return new object[] {typeof(long), typeof(long)};
        yield return new object[] {typeof(ushort), typeof(ushort)};
        yield return new object[] {typeof(uint), typeof(uint)};
        yield return new object[] {typeof(ulong), typeof(ulong)};
        yield return new object[] {typeof(Uri), typeof(Uri)};
      }
    }
  }
}