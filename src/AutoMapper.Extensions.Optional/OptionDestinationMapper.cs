using System;
using System.Linq.Expressions;
using System.Reflection;
using Optional;

namespace AutoMapper.Extensions.Optional
{
  public class OptionDestinationMapper : IObjectMapper
  {
    public bool IsMatch(TypePair context)
    {
      if (context.SourceType.IsGenericType
          && context.SourceType.GetGenericTypeDefinition() == typeof(Option<>))
      {
        return false;
      }

      return context.DestinationType.IsGenericType
             && context.DestinationType.GetGenericTypeDefinition() == typeof(Option<>);
    }

    public Expression MapExpression(
      IConfigurationProvider configurationProvider,
      ProfileMap profileMap,
      IMemberMap memberMap,
      Expression sourceExpression,
      Expression destExpression,
      Expression contextExpression)
    {
      var concreteDestinationType = destExpression.Type.GenericTypeArguments[0];
      
      var mapMethod = GetMapMethod(sourceExpression.Type, concreteDestinationType);
      var mapExpression = Expression.Call(contextExpression, mapMethod, sourceExpression);

      if (sourceExpression.Type.IsGenericType && sourceExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        var toOptionMethod = MakeStaticGeneric(nameof(ToOption), concreteDestinationType);
        
        return Expression.Call(toOptionMethod, sourceExpression);
      }
      
      if (sourceExpression.Type.IsValueType)
      {
        var someMethod = MakeStaticGeneric(nameof(Some), concreteDestinationType);

        return Expression.Call(someMethod, mapExpression);
      }
      
      var someNotNullMethod = MakeStaticGeneric(nameof(SomeNotNull), concreteDestinationType);

      return Expression.Call(someNotNullMethod, mapExpression);
    }

    private static MethodInfo GetMapMethod(Type sourceType, Type destinationType) =>
      typeof(IMapper)
        .GetMethods()[2]
        .MakeGenericMethod(sourceType, destinationType);

    private static Option<T> Some<T>(T value) => value.Some();
    private static Option<T> SomeNotNull<T>(T value) => value.SomeNotNull();
    private static Option<T> ToOption<T>(T? value) where T : struct => value.ToOption();

    private static MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      typeof(OptionDestinationMapper)
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");
  }
}