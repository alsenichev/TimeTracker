using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Utils
{
  /// <summary>
  /// Contains static methods related to <see cref="Result{T}" />
  /// </summary>
  public static class Results
  {
    /// <summary>
    /// Creates a <see cref="Result{T}" /> with a consistent Value of type T.
    /// </summary>
    public static Result<T> Success<T>(T value)
    {
      return new Result<T>(value, true, Enumerable.Empty<string>());
    }

    /// <summary>
    /// Creates a <see cref="Result{T}" /> with a consistent Value of type T and
    /// a collection of messages.
    /// </summary>
    public static Result<T> Success<T>(T value, IEnumerable<string> messages)
    {
      return new Result<T>(value, true, messages);
    }

    /// <summary>
    /// Creates a Result of type <see cref="object"/> that indicates a successful unit operation.
    /// </summary>
    public static readonly Result<object> SuccessfulUnit =
      new Result<object>(new object(), true, Enumerable.Empty<string>());

    /// <summary>
    /// Creates a <see cref="Result{T}" /> with a default value of type T and
    /// a collection of messages. The IsSuccess property will return false.
    /// </summary>
    public static Result<T> Failure<T>(IEnumerable<string> messages)
    {
      if (typeof(T).IsConstructedGenericType)
      {
      }
      return new Result<T>((T) new object(), false, messages);
    }

    /// <summary>
    /// Creates a <see cref="Result{T}" /> with a default value of type T and
    /// a collection of messages containing the specified message.
    /// The IsSuccess property will return false.
    /// </summary>
    public static Result<T> Failure<T>(string message)
    {
      return new Result<T>((T) new object(), false, new[] {message});
    }

    /// <summary>
    /// Applies a function if result is Success.
    /// Merges any existing messages with the new result.
    /// </summary>
    /// <remarks>
    /// Use it to chain Result generating functions together.
    /// </remarks>
    public static Result<TR> Bind<T, TR>(this Result<T> result,
      Func<T, Result<TR>> func)
    {
      if (result.IsSuccess)
      {
        Result<TR> nextResult = func(result.Value);
        IEnumerable<string> combinedMessages = result.Messages.Concat(nextResult.Messages);
        return nextResult.IsSuccess
          ? Success(nextResult.Value, combinedMessages)
          : Failure<TR>(combinedMessages);
      }
      return Failure<TR>(result.Messages);
    }

    /// <summary>
    /// Applies a function if result is Success. Wraps the result into Result.
    /// </summary>
    /// <remarks>
    /// Use it to transform the Result's underlying type to another type.
    /// </remarks>
    public static Result<TR> Map<T, TR>(this Result<T> result, Func<T, TR> func)
    {
      //logically it is the same as Lift with one argument
      //Map is added for convenience as an extension method to Result
      return Lift(func, result);
    }

    /// <summary>
    /// Invokes an Action if result is Success and passes the result through.
    /// </summary>
    /// <remarks>
    /// Use it when you don't care about the result of the method,
    /// for example, logging.
    /// </remarks>
    public static Result<T> Tee<T>(this Result<T> result,
      Action<T> action)
    {
      if (result.IsSuccess)
      {
        action(result.Value);
      }
      return result;
    }

    /// <summary>
    /// Given a function that transforms a value
    /// applies it only if the argument is Success.
    /// </summary>
    /// <remarks>
    /// Use it to ensure that an argument provided to the function is consistent.
    /// </remarks>
    public static Result<TR> Lift<T, TR>(Func<T, TR> func, Result<T> arg)
    {
      if (arg.IsSuccess)
      {
        return Success(func(arg.Value), arg.Messages);
      } else
      {
        return Failure<TR>(arg.Messages);
      }
    }

    /// <summary>
    /// Given a two parameters function
    /// applies it to arguments' values only if they both are Success.
    /// Merges any existing messages.
    /// </summary>
    /// <remarks>
    /// Use it to ensure that all arguments passed to the function are consistent,
    /// and if not, you will get a validation message from every argument that
    /// failed validation.
    /// </remarks>
    public static Result<TR> Lift<T, T1, TR>(Func<T, T1, TR> func, Result<T> arg,
      Result<T1> arg1)
    {
      IEnumerable<string> messages = arg.Messages.Concat(arg1.Messages);
      if (arg.IsSuccess && arg1.IsSuccess)
      {
        return Success(func(arg.Value, arg1.Value), messages);
      } else
      {
        return Failure<TR>(messages);
      }
    }

    /// <summary>
    /// Given a three parameters function
    /// applies it to the arguments' values only if they all are Success.
    /// Merges any existing messages.
    /// </summary>
    /// ///
    /// <remarks>
    /// Use it to ensure that all arguments passed to the function are consistent,
    /// and if not, you will get a validation message from every argument that
    /// failed validation.
    /// </remarks>
    public static Result<TR> Lift<T, T1, T2, TR>(Func<T, T1, T2, TR> func, Result<T> arg,
      Result<T1> arg1, Result<T2> arg2)
    {
      IEnumerable<string> messages =
        arg.Messages.Concat(arg1.Messages).Concat(arg2.Messages);
      if (arg.IsSuccess && arg1.IsSuccess && arg2.IsSuccess)
      {
        return Success(func(arg.Value, arg1.Value, arg2.Value), messages);
      } else
      {
        return Failure<TR>(messages);
      }
    }

    /// <summary>
    /// Given a four parameters' function
    /// applies it to arguments' values only if they all are Success.
    /// Merges any existing messages.
    /// </summary>
    /// ///
    /// <remarks>
    /// Use it to ensure that all arguments passed to the function are consistent,
    /// and if not, you will get a validation message from every argument that
    /// failed validation.
    /// </remarks>
    public static Result<TR> Lift<T, T1, T2, T3, TR>(Func<T, T1, T2, T3, TR> func,
      Result<T> arg,
      Result<T1> arg1, Result<T2> arg2, Result<T3> arg3)
    {
      IEnumerable<string> messages =
        arg.Messages.Concat(arg1.Messages).Concat(arg2.Messages).Concat(arg3.Messages);
      if (arg.IsSuccess && arg1.IsSuccess && arg2.IsSuccess && arg3.IsSuccess)
      {
        return Success(func(arg.Value, arg1.Value, arg2.Value, arg3.Value), messages);
      } else
      {
        return Failure<TR>(messages);
      }
    }
  }

  /// <summary>
  /// Stores the T value and knows whether it is consistent. Also contains information
  /// about error or info messages that have occurred while obtaining the value T.
  /// </summary>
  /// <remarks>
  /// Use this interface to separate exception handling and validation logic
  /// from the domain flow.
  /// Make your functions return Result of T, and chain the results together
  /// using Bind extension method of the <see cref="Results"/> class.
  /// Then you can check the final result for success or failure,
  /// access its value and read error (or info) messages.
  /// </remarks>
  public struct Result<T>
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    public Result(T value, bool isSuccess, IEnumerable<string> messages)
    {
      Value = value;
      IsSuccess = isSuccess;
      Messages = messages;
    }

    /// <summary>
    /// The value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Messages.
    /// </summary>
    public IEnumerable<string> Messages { get; }

    /// <summary>
    /// Indicates that this result is success.
    /// </summary>
    public bool IsSuccess { get; }

    public override string ToString()
    {
      return $"IsSuccess: {IsSuccess}, Value: {Value}, Messages: {string.Join(", ", Messages)}";
    }
  }
}