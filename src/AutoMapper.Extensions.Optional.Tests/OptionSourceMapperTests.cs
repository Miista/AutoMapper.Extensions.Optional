using System;
using System.Collections;
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

    [Theory]
    [ClassData(typeof(TypesData))]
    public void Can_map_from_Option2(Type sourceType, Type destinationType)
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

  public class TypesData : IEnumerable<object[]>
  {
    public IEnumerator<object[]> GetEnumerator()
    {
      // decimal is skipped because it causes an AmbiguousMatchException
      yield return new object[] {typeof(bool), typeof(bool)};
      yield return new object[] {typeof(string), typeof(string)};
      yield return new object[] {typeof(char), typeof(char)};
      yield return new object[] {typeof(byte), typeof(byte)};
      yield return new object[] {typeof(double), typeof(double)};
      yield return new object[] {typeof(double), typeof(string)};
      yield return new object[] {typeof(float), typeof(float)};
      yield return new object[] {typeof(float), typeof(int)};
      yield return new object[] {typeof(short), typeof(short)};
      yield return new object[] {typeof(int), typeof(int)};
      yield return new object[] {typeof(long), typeof(long)};
      yield return new object[] {typeof(ushort), typeof(ushort)};
      yield return new object[] {typeof(uint), typeof(uint)};
      yield return new object[] {typeof(ulong), typeof(ulong)};
      yield return new object[] {typeof(Uri), typeof(Uri)};
      yield return new object[] {typeof(Uri), typeof(string)};
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}