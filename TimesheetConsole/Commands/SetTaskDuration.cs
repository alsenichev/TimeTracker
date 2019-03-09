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
      Result<object> updateTaskDuration(Status status)
      {
        int index = int.Parse(regexMatch.Groups["index"].Value) - 1;
        if (status.Day.Tasks == null || status.Day.Tasks.Count <= index)
        {
          return Results.Failure<object>($"There is no task at index {index + 1} in today's sheet.");
        }
        TimeSpan duration;
        if (regexMatch.Groups["time"].Success)
        {
          int hours = int.Parse(regexMatch.Groups["hours"].Value);
          bool fraction = regexMatch.Groups["fraction"].Success;
          var absDuration = TimeSpan.FromHours(hours);
          if (fraction)
          {
            absDuration += TimeSpan.FromMinutes(30);
          }
          duration = regexMatch.Groups["minus"].Success ? absDuration.Negate() : absDuration;
        }
        else
        {
          duration = status.UnregisteredTime.TaskAssignable();
        }
        return repository.SaveTodaySheet(status.Day.AddToTaskDuration(index, duration));
      }
      return repository.GetStatus()
        .Bind(updateTaskDuration)
        .Bind(_ => todaysSheet.Execute(null));
    }
  }
}