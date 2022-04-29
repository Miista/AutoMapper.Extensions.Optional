using System;
using AutoMapper;
using AutoMapper.Extensions.Optional;
using Optional;

namespace Sandbox
{
  class Program
  {
    static void Main(string[] args)
    {
      var mapperConfiguration = new MapperConfiguration(expression =>
      {
        expression.Mappers.Add(new OptionSourceMapper());
        expression.CreateMap(typeof(Option<>), typeof(Option<>)).ConvertUsing(typeof(Option2OptionTypeMapper<,>));
      });
      var mapper = mapperConfiguration.CreateMapper();
      //Console.WriteLine(mapper.Map<double>(((double) 2).Some()));
      //Console.WriteLine(mapper.Map<double>(Option.None<double>()));
      Console.WriteLine(mapper.Map<int>(true));
      Console.WriteLine(mapper.Map<double>((double?)null));
      Console.WriteLine(mapper.Map<double?>(null));
      Console.WriteLine(mapper.Map<double?>(Option.None<double>()));
      Console.WriteLine(mapper.Map<Option<double>>(((int)2).Some()));
      Console.WriteLine(Option.None<bool?>());
      Console.WriteLine(Option.Some<bool?>(true));
    }
  }
}