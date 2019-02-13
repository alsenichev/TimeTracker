using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Utils
{
  /// <summary>
  /// Contains static and extension methods for Option.
  /// </summary>
  public static class Options
  {
    /// <summary>
    /// Creates the option with an existing value.
    /// </summary>
    public static Option<T> Some<T>(T value)
    {
      return Option<T>.Some(value);
    }

    /// <summary>
    /// Creates the option with a non existing value.
    /// </summary>
    public static Option<T> None<T>()
    {
      return Option<T>.None();
    }

    /// <summary>
    /// Creates Option.Some if the underlying value is not null,
    /// Option.None otherwise.
    /// </summary>
    public static Option<T> NotNull<T>(T value) where T : class
    {
      if (value == null)
      {
        return None<T>();
      }
      else
      {
        return Some(value);
      }
    }

    /// <summary>
    /// Creates and Option.Some if the enumerable is not empty,
    /// Option.None otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static Option<IList<T>> NonEmpty<T>(IEnumerable<T> enumerable)
    {
      var value = enumerable as IList<T> ?? enumerable.ToList();
      if (value.Any())
      {
        return Some(value);
      }
      else
      {
        return None<IList<T>>();
      }
    }

    /// <summary>
    /// Returns Option.Some if the value satisfies the predicate,
    /// Option.None otherwise.
    /// </summary>
    /// <returns></returns>
    public static Option<T> Of<T>(T value, Func<T, bool> predicate)
    {
      if (predicate(value))
      {
        return Some(value);
      }
      else
      {
        return None<T>();
      }
    }

    /// <summary>
    /// Returns the underlying value if Some, default(Value) otherwise.
    /// </summary>
    public static T ValueOrDefault<T>(this Option<T> option)
    {
      if (option.IsSome)
      {
        return option.Value;
      }
      else
      {
        return default(T);
      }
    }

    /// <summary>
    /// Returns the result of fSome function if Some,
    /// the result of fNone function otherwise.
    /// </summary>
    public static TResult Fold<T, TResult>(this Option<T> option, Func<T, TResult> fSome,
      Func<TResult> fNone)
    {
      if (option.IsSome)
      {
        return fSome(option.Value);
      }
      else
      {
        return fNone();
      }
    }

    /// <summary>
    /// Runs fSome action if Some, fNone action otherwise.
    /// </summary>
    public static void Match<T>(this Option<T> option, Action<T> fSome, Action fNone)
    {
      if (option.IsSome)
      {
        fSome(option.Value);
      }
      else
      {
        fNone();
      }
    }

    /// <summary>
    /// Converts an Option of type T to the Option of type TResult with a
    /// given function.
    /// </summary>
    public static Option<TResult> Map<T, TResult>(this Option<T> option,
      Func<T, TResult> fMap)
    {
      return option.IsSome ? Some(fMap(option.Value)) : None<TResult>();
    }

    /// <summary>
    /// Applies the function to the Option if Some.
    /// </summary>
    public static Option<TResult> Bind<T, TResult>(this Option<T> option,
      Func<T, Option<TResult>> fSome)
    {
      Func<Option<TResult>> fNone = () => None<TResult>();
      return option.Fold(fSome, fNone);
    }
  }

  /// <summary>
  /// Stores the T value and indicates that it exists.
  /// Use static methods of the <see cref="Options" /> class to construct
  /// and option.
  /// </summary>
  // ReSharper disable once InconsistentNaming
  // ... we want to hide the constructors of Option behind the interface,
  // but we don't want to attract attention to that fact.
  public struct Option<T>
  {
    #region enums

    /// <summary>
    /// The option can be either Some or None
    /// </summary>
    enum OptionType
    {
      // It is important that None is the first enum
      // because if the struct is not initialized it will have
      // the default value None.
      None,

      Some
    }

    #endregion

    #region private methods

    /// <summary>
    /// We do not expose the constructor.
    /// </summary>
    Option(OptionType optionType, T value)
    {
      this.optionType = optionType;
      this.value = value;
    }

    #endregion

    #region private fields

    readonly OptionType optionType;
    readonly T value;

    #endregion

    #region internal static factory methods

    internal static Option<T> None()
    {
      return new Option<T>(OptionType.None, default(T));
    }

    internal static Option<T> Some(T value)
    {
      return new Option<T>(OptionType.Some, value);
    }

    #endregion

    #region public properties

    /// <summary>
    /// Indicates that the underlying value exists.
    /// </summary>
    public bool IsSome => optionType == OptionType.Some;

    /// <summary>
    /// Indicates that the underlying value does not exist.
    /// </summary>
    public bool IsNone => optionType == OptionType.None;

    /// <summary>
    /// The underlying value.
    /// </summary>
    public T Value => value;

    public override string ToString()
    {
      return $"IsSome: {IsSome}, Value: {Value}";
    }

    #endregion
  }
}