using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  class Stash : AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public Stash(
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
      Result<string> SaveStash(Status status)
      {
        if (status.UnregisteredTime > TimeSpan.FromMinutes(1))
        {
          return repository.SaveStash(status.Stash + status.UnregisteredTime)
            .Bind(_ => todaysSheet.Execute(Program.log.Match("log")));
        }
        else
        {
          return Results.Success("Nothing to stash.");
        }
      }
      return repository.GetStatus().Bind(SaveStash);
    }
  }
}