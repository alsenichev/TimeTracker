using System;
using System.Linq;
using Domain.Utils;

namespace Domain.Models
{
  /// <summary>
  /// Invariants:
  /// 1. Time started is immutable.
  /// 2. Task durations are not negative.
  /// 3. Pause can be negative.
  /// 4. Stash is not negative.
  /// 5. Duration is how many time passed since day started (can only grow).
  /// 6. CleanTime = Duration - Pause (negative Pause adds to CleanTime - this is used for Squirreling)
  /// 7. Stash + Unregistered + Registered = CleanTime
  /// 8. When a new Day is created, Stash becomes a negative pause (this is Squirreling)
  /// </summary>
  public struct Status
  {
    public Status(Day day, TimeSpan stash)
    {
      Stash = stash;
      Day = day;
    }

    public Day Day { get; }

    public TimeSpan Stash { get; }

    public DateTime StartedAt => Day.DayStarted;

    public TimeSpan CleanTime =>
      DateTime.Now.RoundSeconds() - StartedAt - Day.Break;

    public TimeSpan RegisteredTime =>
      Day.Tasks.Aggregate(TimeSpan.Zero, (a, c) => a + c.Duration);

    public TimeSpan UnregisteredTime => CleanTime - RegisteredTime - Stash;

    public TimeSpan Pause => Day.Break;

    public bool IsOvertime => CleanTime > TimeSpan.FromHours(8);

    public TimeSpan Overtime => CleanTime - TimeSpan.FromHours(8);

    public DateTime EndOfDay => StartedAt.Add(TimeSpan.FromHours(8)).Add(Pause);

    public TimeSpan Left => TimeSpan.FromHours(8).Subtract(CleanTime);
  }
}