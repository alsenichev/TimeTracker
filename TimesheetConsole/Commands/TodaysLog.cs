using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using Domain.BusinessRules;
using Domain.Models;

namespace TimesheetConsole.Commands
{
  public static class TodaysLog
  {
    private static string FormatLog(DateTime now, DailyLog log, bool includeHeader, bool includeTasks)
    {
      string startedAt = GetTime.GetLogStatus(now, log);
      string header = includeHeader ? $"{startedAt}{Environment.NewLine}{Environment.NewLine}" : "";
      if (!includeTasks)
      {
          return $"{header}No tasks created today.";
      }
      var formattedEntries = log.TaskEntries.Select((e, i) =>
        $"{i + 1}. {e.Name}, duration: {GetTime.FormatTime(e.Duration)}");
      var entries = string.Join(Environment.NewLine, formattedEntries);
      var total = log.TaskEntries.Aggregate(TimeSpan.Zero, (a, c) => a + c.Duration);
      return $"{header}Here's the list of your tasks:{Environment.NewLine}{entries}{Environment.NewLine}Totally {GetTime.FormatTime(total)}";
    }

    public static string Execute(DateTime now, bool includeHeader)
    {
      var repo = new MainRepository();
      var log = repo.LoadLatestLog();
      if (log != null && TimeManagement.IsToday(log.DayStarted))
      {
        if (log.TaskEntries != null)
        {
          return FormatLog(now, log, includeHeader, true);
        }
        return FormatLog(now, log, includeHeader, false);
      }
      else
      {
        StartWorkingDay.Execute(now);
        log = repo.LoadLatestLog();
        return FormatLog(now, log, includeHeader, false);
      }
    }
  }
}