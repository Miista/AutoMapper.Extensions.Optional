using System;
using System.Linq.Expressions;
using System.Reflection;
using Optional;

namespace AutoMapper.Extensions.Optional
{
  public class OptionSourceMapper : IObjectMapper
  {
    public bool IsMatch(TypePair context)
    {
      if (context.DestinationType.IsGenericType
          && context.DestinationType.GetGenericTypeDefinition() == typeof(Option<>))
      {
        return false;
      }

      return context.SourceType.IsGenericType
             && context.SourceType.GetGenericTypeDefinition() == typeof(Option<>);
    }

    public Expression MapExpression(
      IConfigurationProvider configurationProvider,
      ProfileMap profileMap,
      IMemberMap memberMap,
      Expression sourceExpression,
      Expression destExpression,
      Expression contextExpression)
    {
      var concreteSourceType = sourceExpression.Type.GenericTypeArguments[0];
      var concreteDestinationType = destExpression.Type;
      
      var hasValueMethod = MakeStaticGeneric(nameof(HasValue), concreteSourceType);
      var getValueMethod = MakeStaticGeneric(nameof(GetValue), concreteSourceType);
      var mapMethod = GetMapMethod(concreteSourceType, concreteDestinationType);

      var getValueExpression = Expression.Call(getValueMethod, sourceExpression);
      var mapExpression = Expression.Call(contextExpression, mapMethod, getValueExpression);
      
      return Expression.Condition(
        test: Expression.Call(hasValueMethod, sourceExpression),
        ifTrue: mapExpression,
        ifFalse: Expression.Default(concreteDestinationType)
      );
    }

    private static MethodInfo GetMapMethod(Type sourceType, Type destinationType) =>
      typeof(IMapper)
        .GetMethods()[2]
        .MakeGenericMethod(sourceType, destinationType);

    private static bool HasValue<T>(Option<T> option) => option.HasValue;
    private static T GetValue<T>(Option<T> option) => option.ValueOr(default(T));

    private static MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      typeof(OptionSourceMapper)
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");
  }
}