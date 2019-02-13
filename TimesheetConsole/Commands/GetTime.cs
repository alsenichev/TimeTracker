using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.BusinessRules;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class GetTime : AppCommandBase
  {
    private readonly MainRepository repository;

    public static string FormatTime(TimeSpan time)
    {
      return $"{time.Hours}h {time.Minutes}m";
    }

    private static string MoreToWork(DateTime started, TimeSpan passed, TimeSpan left)
    {
      return
        $@"{Program.ProductOwner}, {FormatTime(passed)} have passed since you've started today at {started:t}.
         More {FormatTime(left)} to go for a 8 hours working day.";
    }

    private static string Overtime(DateTime started, TimeSpan passed)
    {
      return
        $"{Program.ProductOwner}, {FormatTime(passed)} have passed since you've started today at {started:t}.";
    }

    private static string NotStarted()
    {
      return $"{Program.ProductOwner}, you've not started yet :)";
    }

    public static string GetLogStatus(DailySheet sheet)
    {
      DateTime started = sheet.DayStarted;
      TimeSpan passed = TimeManagement.PassedSince(started, DateTime.Now);
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

    public GetTime(string name, Regex regex, MainRepository repository) : base(name, regex)
    {
      this.repository = repository;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      return repository.GetTodaySheet()
        .Map(o => o.Fold(GetLogStatus, NotStarted));
    }
  }
}