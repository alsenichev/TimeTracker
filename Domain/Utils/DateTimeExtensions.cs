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

  }
}