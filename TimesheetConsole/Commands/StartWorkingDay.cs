using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class StartWorkingDay : AppCommandBase
  {
    private readonly MainRepository repository;

    private static string WorkingDayStarted(DateTime time)
    {
      return $"Hello {Program.ProductOwner}. Your working day started {time:f}";
    }

    private static string WorkingDayAlreadyStarted(DateTime time)
    {
      return $"{Program.ProductOwner}, your working day is already started at {time:f}";
    }

    public StartWorkingDay(string name, Regex regex, MainRepository repository) : base(name, regex)
    {
      this.repository = repository;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      Result<string> todayNotPresent() =>
        repository.SaveTodaySheet(new DailySheet {DayStarted = DateTime.Now})
          .Map(r => WorkingDayStarted(DateTime.Now));

      Result<string> todayPresent(DailySheet s) =>
        Results.Success(WorkingDayAlreadyStarted(s.DayStarted));

      Result<string> startIfTodayNotPresent(Option<DailySheet> maybeToday) =>
        maybeToday.Fold(todayPresent, todayNotPresent);

      return repository.GetTodaySheet().Bind(startIfTodayNotPresent);
    }
  }
}