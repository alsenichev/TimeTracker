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
    /// Creates and Option.Some if the enumerable is not empty,
    /// Option.None otherwise.
    /// </summary>
    public static Option<IList<T>> NonEmpty<T>(IEnumerable<T> enumerable)
    {
      var value = enumerable as IList<T> ?? enumerable.ToList();
      return value.Any() ? Some(value) : None<IList<T>>();
    }

    /// <summary>
    /// Returns Option.Some if the value satisfies the predicate,
    /// Option.None otherwise.
    /// </summary>
    /// <returns></returns>
    public static Option<T> Of<T>(T value, Func<T, bool> predicate)
    {
      return predicate(value) ? Some(value) : None<T>();
    }

    /// <summary>
    /// Returns the underlying value if Some, default(Value) otherwise.
    /// </summary>
    public static T ValueOrDefault<T>(this Option<T> option)
    {
      return option.IsSome ? option.Value : default;
    }

    /// <summary>
    /// Returns the result of fSome function if Some,
    /// the result of fNone function otherwise.
    /// </summary>
    public static TResult Fold<T, TResult>(this Option<T> option, Func<T, TResult> fSome,
      Func<TResult> fNone)
    {
      return option.IsSome ? fSome(option.Value) : fNone();
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
      Option<TResult> fNone() => None<TResult>();
      return option.Fold(fSome, fNone);
    }
  }

  /// <summary>
  /// Stores the T value and indicates that it exists.
  /// Use static methods of the <see cref="Options" /> class to construct
  /// the option.
  /// </summary>
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
    private Option(OptionType optionType, T value)
    {
      this.optionType = optionType;
      this.Value = value;
    }

    #endregion

    #region private fields

    private readonly OptionType optionType;

    #endregion

    #region internal static factory methods

    internal static Option<T> None()
    {
      return new Option<T>(OptionType.None, default);
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
    public T Value { get; }

    public override string ToString()
    {
      return $"IsSome: {IsSome}, Value: {Value}";
    }

    #endregion
  }
}