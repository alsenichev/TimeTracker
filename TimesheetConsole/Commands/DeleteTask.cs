using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class DeleteTask:AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public DeleteTask(
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
      int index = int.Parse(regexMatch.Groups["index"].Value) - 1;
      Result<object> deleteTask(DailySheet sheet)
      {
        if (sheet.TaskEntries==null || sheet.TaskEntries.Count <= index)
        {
          return Results.Failure<object>($"There is no task at index {index + 1} in today's sheet.");
        }
        sheet.TaskEntries.RemoveAt(index);
        return repository.SaveTodaySheet(sheet);
      }

      return repository.GetTodaySheet()
        .Bind(o => o.Fold(
          deleteTask,
          () => Results.Failure<object>("There is no today's sheet to delete a task.")))
        .Bind(_ => todaysSheet.Execute(null));
    }
  }
}