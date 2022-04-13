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
      });
      var mapper = mapperConfiguration.CreateMapper();
      //Console.WriteLine(mapper.Map<double>(((double) 2).Some()));
      //Console.WriteLine(mapper.Map<double>(Option.None<double>()));
      Console.WriteLine(mapper.Map<double?>(null));
      Console.WriteLine(mapper.Map<double?>(Option.None<double>()));
    }
  }
}