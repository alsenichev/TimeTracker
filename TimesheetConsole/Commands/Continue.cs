using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  class Continue : AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public Continue(
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
      Result<object> flushToPause(Status status)
      {
        return repository.SaveTodaySheet(status.Day.AddToPause(status.UnregisteredTime));
      }

      return repository.GetStatus()
        .Bind(s => flushToPause(s))
        .Bind(_ => repository.SaveStash(TimeSpan.Zero))
        .Bind(_ => todaysSheet.ExecuteWithHeader());
    }
  }
}