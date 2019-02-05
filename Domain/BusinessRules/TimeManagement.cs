using System;

namespace Domain.BusinessRules
{
  public class TimeManagement
  {
    public static bool IsToday(DateTime time)
    {
      return time.Date.Equals(DateTime.Now.Date);
    }

    public static TimeSpan PassedSince(DateTime started, DateTime now)
    {
      return now - started;
    }

    public static TimeSpan IsLeft(TimeSpan passed, out bool overtime)
    {
      if (passed.TotalHours > 8)
      {
        overtime = true;
        return passed - TimeSpan.FromHours(8);
      }

      overtime = false;
      return TimeSpan.FromHours(8) - passed;
    }
  }
}