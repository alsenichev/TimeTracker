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
      Result<object> deleteTask(Day sheet)
      {
        if (sheet.Tasks==null || sheet.Tasks.Count <= index)
        {
          return Results.Failure<object>($"There is no task at index {index + 1} in today's sheet.");
        }
        sheet.Tasks.RemoveAt(index);
        return repository.SaveTodaySheet(sheet);
      }

      return repository.GetStatus()
        .Bind(s => deleteTask(s.Day))
        .Bind(_ => todaysSheet.ExecuteWithNoHeader());
    }
  }
}