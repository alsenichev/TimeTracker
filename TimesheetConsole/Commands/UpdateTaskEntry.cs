using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.BusinessRules;

namespace TimesheetConsole.Commands
{
  public static class UpdateTaskEntry
  {
    private static readonly Regex exit = new Regex(@"exit");
    private static readonly Regex delete = new Regex(@"(?<index>\d{1,3})\.?\s*del");
    private static readonly Regex duration =
      new Regex(@"(?<index>\d{1,3})\.?\s*(?<hours>\d{1,3})(?<fraction>\.5)?");
    private static readonly Regex add = new Regex(@"add\s*(?<entry>.*)");

    public static (bool success, bool exit) Execute(string userInput)
    {
      if (exit.IsMatch(userInput))
      {
        return (success: true, exit: true);
      }

      if (delete.IsMatch(userInput))
      {
        bool deleteResult=DeleteEntry(int.Parse(delete.Match(userInput).Groups["index"].Value) - 1);
        return (success: deleteResult, exit: false);
      }

      var durationMatch = duration.Match(userInput);
      if (durationMatch.Success)
      {
        int index = int.Parse(durationMatch.Groups["index"].Value) - 1;
        int hours = int.Parse(durationMatch.Groups["hours"].Value);
        bool fraction = durationMatch.Groups["fraction"].Success;
        UpdateDuration(index, hours, fraction);
        return (success: true, exit: false);
      }

      var addMatch = add.Match(userInput);
      if (addMatch.Success)
      {
        string taskEntry = addMatch.Groups["entry"].Value;
        AddTask.Execute(taskEntry.Split(' '));
        return (success: true, exit: false);
      }

      return (success: false, exit: false);
    }

    private static bool UpdateDuration(int index, int hours, bool fraction)
    {
      var repo = new MainRepository();
      var log = repo.LoadLatestLog();
      if (!TimeManagement.IsToday(log.DayStarted))
      {
        // todo move to repository
        throw new ApplicationException("Today's log is expected.");
      }
      try
      {
        log.TaskEntries[index].Duration = TimeSpan.FromHours(hours);
        if (fraction)
        {
          log.TaskEntries[index].Duration += TimeSpan.FromMinutes(30);
        }
      }
      catch (ArgumentOutOfRangeException)
      {
        return false;
      }
      repo.UpdateTodaysLog(log);
      return true;
    }

    private static bool DeleteEntry(int i)
    {
      var repo = new MainRepository();
      var log = repo.LoadLatestLog();
      if (!TimeManagement.IsToday(log.DayStarted))
      {
        // todo move to repository
        throw new ApplicationException("Today's log is expected.");
      }
      try
      {
        log.TaskEntries.RemoveAt(i);
      }
      catch (ArgumentOutOfRangeException)
      {
        return false;
      }
      repo.UpdateTodaysLog(log);
      return true;
    }
  }
}