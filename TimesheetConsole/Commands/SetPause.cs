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
      Result<object> addToPause(Day sheet, TimeSpan p)
      {
        return repository.SaveTodaySheet(sheet.AddToPause(p));
      }
      TimeSpan absPause = TimeSpan.FromMinutes(int.Parse(regexMatch.Groups["minutes"].Value));
      TimeSpan pause = regexMatch.Groups["minus"].Success ? absPause.Negate() : absPause;
      return repository.GetStatus()
        .Bind(s => addToPause(s.Day, pause))
        .Bind(_ => todaysSheet.Execute(Program.log.Match("log")));//Todo make normal parameters in Execute methods
    }
  }
}