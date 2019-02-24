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

    private static string MoreToWork(DateTime started, TimeSpan passed, TimeSpan pause, TimeSpan left, DateTime endOfDay)
    {
      string pausePostfix =
        pause.Equals(TimeSpan.Zero) ? string.Empty : $" and took a {FormatTime(pause)} pause";
      return
        $@"{Program.ProductOwner}, you work for {FormatTime(passed - pause)}.{Environment.NewLine}You've started today at {started:t}{pausePostfix}.{Environment.NewLine}Your working day ends at {endOfDay:t} in {FormatTime(left)}.";
    }

    private static string Overtime(DateTime started, TimeSpan passed, TimeSpan pause, TimeSpan overtime)
    {
      string pausePostfix =
        pause.Equals(TimeSpan.Zero) ? string.Empty : $" and took a {FormatTime(pause)} pause";
      return
        $"{Program.ProductOwner}, you work for {FormatTime(passed - pause)}.{Environment.NewLine}You've started today at {started:t}{pausePostfix}.{Environment.NewLine}It makes a {overtime:t} overtime.";
    }

    private static string NotStarted()
    {
      return $"{Program.ProductOwner}, you've not started yet :)";
    }

    public static string GetLogStatus(DailySheet sheet)
    {
      DateTime started = sheet.DayStarted;
      TimeSpan pause = sheet.Break;
      TimeSpan passed = TimeManagement.PassedSince(started);
      TimeSpan delta = TimeManagement.Delta(passed, pause, out bool overflow);
      DateTime endOfDay = TimeManagement.EndOfDay(started, pause);
      if (overflow)
      {
        return Overtime(started, passed, pause, delta);
      }
      else
      {
        return MoreToWork(started, passed, pause, delta, endOfDay);
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