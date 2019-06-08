using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  class Expend : AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public Expend(
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
        return repository.SaveStash(TimeSpan.Zero)
          .Bind(_ => todaysSheet.ExecuteWithHeader());
      }
      return repository.GetStatus().Bind(SaveStash);
    }
  }
}