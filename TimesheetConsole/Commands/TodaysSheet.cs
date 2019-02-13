using System;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.BusinessRules;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class TodaysSheet : AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly StartWorkingDay startWorkingDay;

    private static string FormatLog(DailySheet sheet, bool includeHeader)
    {
      string startedAt = GetTime.GetLogStatus(sheet);
      string header = includeHeader ? $"{startedAt}{Environment.NewLine}{Environment.NewLine}" : "";
      TimeSpan passed = TimeManagement.PassedSince(sheet.DayStarted);
      if (sheet.TaskEntries == null || sheet.TaskEntries.Count == 0)
      {
        return $"{header}No tasks created today.{Environment.NewLine}Unregistered time {GetTime.FormatTime(passed - sheet.Break)}.";
      }

      var formattedEntries = sheet.TaskEntries.Select((e, i) =>
        $"{i + 1}. {e.Name}, duration: {GetTime.FormatTime(e.Duration)}");
      var entries = string.Join(Environment.NewLine, formattedEntries);
      var total = sheet.TaskEntries.Aggregate(TimeSpan.Zero, (a, c) => a + c.Duration);
      return
        $"{header}Here's the list of your tasks:{Environment.NewLine}{entries}{Environment.NewLine}Totally {GetTime.FormatTime(total)}. Unregistered time {GetTime.FormatTime(passed-sheet.Break-total)}.";
    }

    public TodaysSheet(
      string name, Regex regex, MainRepository repository, StartWorkingDay startWorkingDay) : base(
      name, regex)
    {
      this.repository = repository;
      this.startWorkingDay = startWorkingDay;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      // todo - a bit of a hack here. to make it correct we would need an Execute<T> signature
      bool displayHeader = regexMatch != null;
      Result<string> formatSheet(DailySheet sheet) =>
        Results.Success(FormatLog(sheet, displayHeader));

      Result<string> startAndFormat() =>
        startWorkingDay.Execute(null)
          .Bind(_ => repository.GetTodaySheet())
          .Bind(o => o.Fold(
            formatSheet,
            () => Results.Failure<string>("Failed to start working day.")));

      return repository.GetTodaySheet().Bind(o => o.Fold(formatSheet, startAndFormat));
    }
  }
}