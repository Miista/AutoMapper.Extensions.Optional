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
      
      var someMethod = MakeStaticGeneric(nameof(Some), concreteDestinationType);
      var noneMethod = MakeStaticGeneric(nameof(None), concreteDestinationType);
      var mapMethod = GetMapMethod(sourceExpression.Type, concreteDestinationType);
      
      var mapExpression = Expression.Call(contextExpression, mapMethod, sourceExpression);
      
      if (sourceExpression.Type.IsValueType)
      {
        return Expression.Call(someMethod, mapExpression);
      }
      
      var nullCheck = Expression.Equal(
        left: Expression.Constant(null),
        right: sourceExpression
      );

      var conditionalExpression = Expression.Condition(
        test: nullCheck,
        ifTrue: Expression.Call(noneMethod),
        ifFalse: Expression.Call(someMethod, mapExpression)
      );

      return conditionalExpression;
    }

    private static MethodInfo GetMapMethod(Type sourceType, Type destinationType) =>
      typeof(IMapper)
        .GetMethods()[2]
        .MakeGenericMethod(sourceType, destinationType);

    private static Option<T> Some<T>(T value) => value.Some();
    private static Option<T> None<T>() => Option.None<T>();

    private static MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      typeof(OptionDestinationMapper)
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");
  }
}