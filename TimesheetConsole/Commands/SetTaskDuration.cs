using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class SetTaskDuration:AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public SetTaskDuration(
      string name,
      Regex regex,
      MainRepository repository,
      TodaysSheet todaysSheet) : base(name, regex)
    {
      this.repository = repository;
      this.todaysSheet = todaysSheet;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      Result<object> updateTaskDuration(Day day)
      {
        int index = int.Parse(regexMatch.Groups["index"].Value) - 1;
        if (day.Tasks == null || day.Tasks.Count <= index)
        {
          return Results.Failure<object>($"There is no task at index {index + 1} in today's sheet.");
        }
        int hours = int.Parse(regexMatch.Groups["hours"].Value);
        bool fraction = regexMatch.Groups["fraction"].Success;
        var absDuration = TimeSpan.FromHours(hours);
        if (fraction)
        {
          absDuration += TimeSpan.FromMinutes(30);
        }
        var duration = regexMatch.Groups["minus"].Success ? absDuration.Negate() : absDuration;
        return repository.SaveTodaySheet(day.AddToTaskDuration(index, duration));
      }
      return repository.GetStatus()
        .Bind(s => updateTaskDuration(s.Day))
        .Bind(_ => todaysSheet.Execute(null));
    }
  }
}