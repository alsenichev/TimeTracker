using System;

namespace Domain.Utils
{
  public static class DateTimeExtensions
  {
    private static DateTime RoundToNearest(DateTime dt, TimeSpan d)
    {
      var delta = dt.Ticks % d.Ticks;
      bool roundUp = delta > d.Ticks / 2;
      var offset = roundUp ? d.Ticks : 0;
      return new DateTime(dt.Ticks + offset - delta, dt.Kind);
    }

    public static DateTime RoundSeconds(this DateTime dt)
    {
      return RoundToNearest(dt, TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// Old way: returns the part of the TimeSpan which is a multiple of 30 minutes.
    /// Modern way: rounds up to the nearest 30 minutes.
    /// </summary>
    public static TimeSpan TaskAssignable(this TimeSpan ts)
    {
      TimeSpan step = TimeSpan.FromMinutes(30);
      // 40|50
      // delta = 10|20
      var delta = ts.Ticks % step.Ticks;
      // false|true
      bool roundUp = delta > step.Ticks / 2;
      // 0|30
      var offset = roundUp ? step.Ticks : 0;
      // 40 + 0 - 10 = 30 | 50 + 30 - 20 = 60
      return TimeSpan.FromTicks(ts.Ticks + offset - delta);
    }

  }
}