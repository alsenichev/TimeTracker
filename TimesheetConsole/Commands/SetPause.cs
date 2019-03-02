using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  class SetPause : AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public SetPause(
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
      Result<object> setPause(Day sheet, TimeSpan p)
      {
        return repository.SaveTodaySheet(sheet.SetPause(p));
      }
      TimeSpan pause = TimeSpan.FromMinutes(int.Parse(regexMatch.Groups["minutes"].Value));
      return repository.GetStatus()
        .Bind(s => setPause(s.Day, pause))
        .Bind(_ => todaysSheet.Execute(Program.log.Match("log")));//Todo make normal parameters in Execute methods
    }
  }
}