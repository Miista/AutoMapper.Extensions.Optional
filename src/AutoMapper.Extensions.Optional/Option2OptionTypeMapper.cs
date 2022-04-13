using Optional;

namespace AutoMapper.Extensions.Optional
{
  public class Option2OptionTypeMapper<TSource, TDestination> : ITypeConverter<Option<TSource>, Option<TDestination>>
  {
    public Option<TDestination> Convert(Option<TSource> source, Option<TDestination> destination, ResolutionContext context)
    {
      return source.Map(value => context.Mapper.Map<TDestination>(value));
    }
  }
}