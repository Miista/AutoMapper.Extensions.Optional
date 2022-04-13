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
      var concreteType = sourceExpression.Type.GenericTypeArguments[0];
      var c = destExpression.Type;
      var hasValueMethod = MakeStaticGeneric(nameof(HasValue), concreteType);
      var getValueMethod = MakeStaticGeneric(nameof(GetValue), concreteType);
      var objectMapper = configurationProvider.FindMapper(new TypePair(concreteType, destExpression.Type));
      var mapMethod = typeof(IObjectMapper).GetMethod(nameof(IObjectMapper.MapExpression));

      //var methodCallExpression = Expression.Call(mapMethod, Expression.Constant(objectMapper), Expression.Constant(configurationProvider),
        //Expression.Constant(profileMap), Expression.Call(getValueMethod, sourceExpression), destExpression, contextExpression);
      return Expression.Condition(
        test: Expression.Call(hasValueMethod, sourceExpression),
        ifTrue: Expression.Call(getValueMethod, sourceExpression),
        ifFalse: Expression.Default(typeof(double))
      );
    }

    private static bool HasValue<T>(Option<T> option) => option.HasValue;
    private static T GetValue<T>(Option<T> option) => option.ValueOr(default(T));

    private static MethodInfo MakeStaticGeneric(string methodName, Type genericType)
    {
      return
        typeof(OptionSourceMapper)
          ?.GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
          ?.MakeGenericMethod(genericType)
        ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");
    }
  }
}