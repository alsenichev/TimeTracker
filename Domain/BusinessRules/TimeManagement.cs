using System;

namespace Domain.BusinessRules
{
  public class TimeManagement
  {
    public static bool IsToday(DateTime time)
    {
      return time.Date.Equals(DateTime.Now.Date);
    }

    public static TimeSpan PassedSince(DateTime started)
    {
      return DateTime.Now - started;
    }

    /// <summary>
    /// Either how much is left or how much is an overtime.
    /// </summary>
    public static TimeSpan Delta(TimeSpan passed, TimeSpan pause, out bool overtime)
    {
      if ((passed - pause).TotalHours > 8)
      {
        overtime = true;
        return passed - pause - TimeSpan.FromHours(8);
      }

      overtime = false;
      return TimeSpan.FromHours(8) + pause - passed;
    }

    public static DateTime EndOfDay(DateTime started, TimeSpan pause)
    {
      return started.Add(TimeSpan.FromHours(8)).Add(pause);
    }
  }
}