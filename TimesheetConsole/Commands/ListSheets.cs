using System;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  class ListSheets : AppCommandBase
  {
    private readonly MainRepository repository;

    private static string FormatLog(DailySheet sheet)
    {
      string header = $"Started on {sheet.DayStarted:D} at {sheet.DayStarted:t}";
      // todo - maybe consider Option for TaskEntries property, but possibly would need a separate model for Json serialization
      if (sheet.TaskEntries == null || sheet.TaskEntries.Count == 0)
      {
        return $"{header}{Environment.NewLine}No tasks created.";
      }

      var formattedEntries = sheet.TaskEntries.Select((e, i) =>
        $"{i + 1}. {e.Name}, duration: {GetTime.FormatTime(e.Duration)}");
      var entries = string.Join(Environment.NewLine, formattedEntries);
      var total = sheet.TaskEntries.Aggregate(TimeSpan.Zero, (a, c) => a + c.Duration);
      return
        $"{header}{Environment.NewLine}{entries}{Environment.NewLine}Totally {GetTime.FormatTime(total)}";
    }

    public ListSheets(string name, Regex regex, MainRepository repository) : base(name, regex)
    {
      this.repository = repository;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      var countGroup = regexMatch.Groups["count"];
      int count = 31;
      if (countGroup.Success)
      {
        count = int.Parse(countGroup.Value);
      }

      return repository.GetAllSheets()
        .Map(s =>
          string.Join($"{Environment.NewLine}{Environment.NewLine}",
            s.OrderByDescending(e => e.DayStarted)
             .Take(count)
             .OrderBy(e => e.DayStarted)
             .Select(e => FormatLog(e))));
    }
  }
}