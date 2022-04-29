using System;
using System.Collections;
using System.Collections.Generic;

namespace AutoMapper.Extensions.Optional.Tests
{
  public class TypesData : IEnumerable<object[]>
  {
    public IEnumerator<object[]> GetEnumerator()
    {
      // decimal is skipped because it causes an AmbiguousMatchException
      yield return new object[] {typeof(bool?), typeof(bool?)};
      yield return new object[] {typeof(double?), typeof(double)};
      yield return new object[] {typeof(double), typeof(double?)};
      yield return new object[] {typeof(bool), typeof(bool)};
      yield return new object[] {typeof(bool), typeof(string)};
      yield return new object[] {typeof(string), typeof(string)};
      yield return new object[] {typeof(char), typeof(char)};
      yield return new object[] {typeof(byte), typeof(byte)};
      yield return new object[] {typeof(double), typeof(double)};
      yield return new object[] {typeof(double), typeof(float)};
      yield return new object[] {typeof(double), typeof(int)};
      yield return new object[] {typeof(double), typeof(string)};
      yield return new object[] {typeof(float), typeof(double)};
      yield return new object[] {typeof(float), typeof(float)};
      yield return new object[] {typeof(float), typeof(int)};
      yield return new object[] {typeof(short), typeof(short)};
      yield return new object[] {typeof(int), typeof(int)};
      yield return new object[] {typeof(int), typeof(double)};
      yield return new object[] {typeof(int), typeof(float)};
      yield return new object[] {typeof(int), typeof(string)};
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