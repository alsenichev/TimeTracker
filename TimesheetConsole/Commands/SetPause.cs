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
      TimeSpan pause = TimeSpan.FromMinutes(int.Parse(regexMatch.Groups["minutes"].Value));
      Result<object> setPause(DailySheet sheet)
      {
        sheet.Break = pause;
        return repository.SaveTodaySheet(sheet);
      }
      return repository.GetTodaySheet()
        .Bind(o => o.Fold(
          setPause,
          ()=>Results.Failure<object>("No today's sheet available to set the pause.")))
        .Bind(_ => todaysSheet.Execute(Program.log.Match("log")));//Todo make normal parameters in Execute methods
    }
  }
}