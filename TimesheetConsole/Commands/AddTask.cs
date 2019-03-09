using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class AddTask:AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public AddTask(
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
      Result<object> createTask(Status status)
      {
        var day = status.Day.AddTask(
          new TaskEntry(regexMatch.Groups["entry"].Value, TimeSpan.Zero));
        return repository.SaveTodaySheet(day);
      }

      return repository.GetStatus()
        .Bind(createTask).Bind(_ => todaysSheet.ExecuteWithNoHeader());
    }
  }
}