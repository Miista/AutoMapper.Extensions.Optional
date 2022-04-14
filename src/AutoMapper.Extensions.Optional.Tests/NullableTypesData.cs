using System.Collections;
using System.Collections.Generic;

namespace AutoMapper.Extensions.Optional.Tests
{
  public class NullableTypesData : IEnumerable<object[]>
  {
    public IEnumerator<object[]> GetEnumerator()
    {
      yield return new object[] {typeof(bool?)};
      yield return new object[] {typeof(char?)};
      yield return new object[] {typeof(double?)};
      yield return new object[] {typeof(decimal?)};
      yield return new object[] {typeof(float?)};
      yield return new object[] {typeof(int?)};
      yield return new object[] {typeof(long?)};
      yield return new object[] {typeof(short?)};
      yield return new object[] {typeof(string)};
      yield return new object[] {typeof(uint?)};
      yield return new object[] {typeof(ulong?)};
      yield return new object[] {typeof(ushort?)};
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}