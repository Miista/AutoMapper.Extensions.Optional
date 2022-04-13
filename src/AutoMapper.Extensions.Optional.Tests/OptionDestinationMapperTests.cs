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

    [Theory]
    [MemberData(nameof(Can_map_to_Option_Data))]
    public void Can_map_to_Option(Type type)
    {
      // Arrange
      var source = _fixture.Create(type, new SpecimenContext(_fixture));
      var destination = CreateOption(source, type);

      var mapperConfiguration = new MapperConfiguration(expression =>
      {
        // The mapper must be inserted at the beginning of the list to ensure that it is visited first.
        expression.Mappers.Insert(0, new OptionDestinationMapper());
      });
      var mapper = mapperConfiguration.CreateMapper();

      // Act
      Action act = () => mapper.Map(source, destination);

      // Assert
      act.Should().NotThrow();

      var resultingValue = mapper.Map(source, destination);
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
        yield return new object[] {typeof(bool)};
        yield return new object[] {typeof(string)};
        yield return new object[] {typeof(char)};
        yield return new object[] {typeof(byte)};
        yield return new object[] {typeof(double)};
        yield return new object[] {typeof(float)};
        yield return new object[] {typeof(short)};
        yield return new object[] {typeof(int)};
        yield return new object[] {typeof(long)};
        yield return new object[] {typeof(ushort)};
        yield return new object[] {typeof(uint)};
        yield return new object[] {typeof(ulong)};
        yield return new object[] {typeof(Uri)};
      }
    }
  }
}