using System;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class TodaysSheet : AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly StartWorkingDay startWorkingDay;

    private static string FormatLog(DailySheet sheet, bool includeHeader, bool includeTasks)
    {
      string startedAt = GetTime.GetLogStatus(sheet);
      string header = includeHeader ? $"{startedAt}{Environment.NewLine}{Environment.NewLine}" : "";
      if (!includeTasks)
      {
        return $"{header}No tasks created today.";
      }

      var formattedEntries = sheet.TaskEntries.Select((e, i) =>
        $"{i + 1}. {e.Name}, duration: {GetTime.FormatTime(e.Duration)}");
      var entries = string.Join(Environment.NewLine, formattedEntries);
      var total = sheet.TaskEntries.Aggregate(TimeSpan.Zero, (a, c) => a + c.Duration);
      return
        $"{header}Here's the list of your tasks:{Environment.NewLine}{entries}{Environment.NewLine}Totally {GetTime.FormatTime(total)}";
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
      Result<string> formatSheet(DailySheet sheet) =>
        sheet.TaskEntries != null
          ? Results.Success(FormatLog(sheet, true, true))
          : Results.Success(FormatLog(sheet, true, false));

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