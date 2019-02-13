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
      Result<object> updateTaskDuration(DailySheet sheet)
      {
        int index = int.Parse(regexMatch.Groups["index"].Value) - 1;
        if (sheet.TaskEntries == null || sheet.TaskEntries.Count <= index)
        {
          return Results.Failure<object>($"There is no task at index {index + 1} in today's sheet.");
        }
        int hours = int.Parse(regexMatch.Groups["hours"].Value);
        bool fraction = regexMatch.Groups["fraction"].Success;
        sheet.TaskEntries[index].Duration = TimeSpan.FromHours(hours);
        if (fraction)
        {
          sheet.TaskEntries[index].Duration += TimeSpan.FromMinutes(30);
        }
        return repository.SaveTodaySheet(sheet);
      }
      return repository.GetTodaySheet()
        .Bind(o => o.Fold(
          updateTaskDuration,
          () => Results.Failure<object>("Today's sheet is expected to delete a task.")))
        .Bind(_ => todaysSheet.Execute(null));


    }
  }
}