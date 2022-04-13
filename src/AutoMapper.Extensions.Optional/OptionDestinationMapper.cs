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
      var concreteType = destExpression.Type.GenericTypeArguments[0];
      var someMethod = MakeStaticGeneric(nameof(Some), concreteType);
      var noneMethod = MakeStaticGeneric(nameof(None), concreteType);

      if (sourceExpression.Type.IsValueType)
      {
        var getMapperExpression = Expression.Property(contextExpression, typeof(ResolutionContext).GetProperty(nameof(ResolutionContext.Mapper)));
        var mapMethodInfo = typeof(IRuntimeMapper).GetMethods()[2].MakeGenericMethod(sourceExpression.Type, concreteType);
        var mapExpression = Expression.Call(contextExpression, typeof(IMapper).GetMethods()[2].MakeGenericMethod(sourceExpression.Type, concreteType), sourceExpression);
        
        return Expression.Call(someMethod, mapExpression);
      }
      
      var nullCheck = Expression.Equal(
        left: Expression.Constant(null),
        right: sourceExpression
      );

      var mapExpression1 = Expression.Call(contextExpression, typeof(IMapper).GetMethods()[2].MakeGenericMethod(sourceExpression.Type, concreteType), sourceExpression);
      
      var conditionalExpression = Expression.Condition(
        test: nullCheck,
        ifTrue: Expression.Call(noneMethod),
        ifFalse: Expression.Call(someMethod, mapExpression1)
      );

      return conditionalExpression;
    }

    private static Option<T> Some<T>(T value) => value.Some();
    private static Option<T> None<T>() => Option.None<T>();

    private static MethodInfo MakeStaticGeneric(string methodName, Type genericType) =>
      typeof(OptionDestinationMapper)
        .GetMethod(methodName, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
        ?.MakeGenericMethod(genericType)
      ?? throw new InvalidOperationException($"Cannot make generic method of '{methodName}'");
  }
}