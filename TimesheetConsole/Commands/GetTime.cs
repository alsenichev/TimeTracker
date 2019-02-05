using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using Domain.BusinessRules;

namespace TimesheetConsole.Commands
{
  public static class GetTime
  {
    private static string FormatTime(TimeSpan time)
    {
      return $"{time.Hours}h {time.Minutes}m";
    }
    private static string MoreToWork(DateTime started, TimeSpan passed, TimeSpan left) => 
      $@"{Program.ProductOwner}, {FormatTime(passed)} passed since you've started today at {started:t}.
         More {FormatTime(left)} to go for a 8 hours working day.";

    private static string Overtime(DateTime started, TimeSpan passed) => 
      $"{Program.ProductOwner}, {FormatTime(passed)} have passed since you've started today at {started:t}.";

    private static string NotStarted() => $"{Program.ProductOwner}, you've not started yet :)";

    public static string Execute(DateTime now)
    {
      var repo = new MainRepository();
      var logs = repo.LoadLogs().OrderByDescending(l => l.DayStarted).ToList();
      if (logs.Count == 0 || !TimeManagement.IsToday(logs[0].DayStarted))
      {
        return NotStarted();
      }

      DateTime started = logs[0].DayStarted;
      TimeSpan passed = TimeManagement.PassedSince(started, now);
      TimeSpan left = TimeManagement.IsLeft(passed, out bool overflow);
      if (overflow)
      {
        return Overtime(started, passed);
      }
      else
      {
        return MoreToWork(started, passed, left);
      }
    }
  }
}
