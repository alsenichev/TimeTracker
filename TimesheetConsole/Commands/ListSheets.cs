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

    private static string FormatLog(Day day)
    {
      string header = $"Started on {day.DayStarted:D} at {day.DayStarted:t}";
      if (day.Tasks.Count == 0)
      {
        return $"{header}{Environment.NewLine}No tasks created.";
      }

      var formattedEntries = day.Tasks.Select((e, i) =>
        $"{i + 1}. {e.Name}, duration: {TodaysSheet.FormatTimeSpan(e.Duration)}");
      var entries = string.Join(Environment.NewLine, formattedEntries);
      var total = day.Tasks.Aggregate(TimeSpan.Zero, (a, c) => a + c.Duration);
      return
        $"{header}{Environment.NewLine}{entries}{Environment.NewLine}Totally {TodaysSheet.FormatTimeSpan(total)}";
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

      return repository.GetAllDays()
        .Map(s =>
          string.Join($"{Environment.NewLine}{Environment.NewLine}",
            s.OrderByDescending(e => e.DayStarted)
             .Take(count)
             .OrderBy(e => e.DayStarted)
             .Select(e => FormatLog(e))));
    }
  }
}