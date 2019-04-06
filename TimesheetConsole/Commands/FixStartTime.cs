using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  class FixStartTime : AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly TodaysSheet todaysSheet;

    public FixStartTime(
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
      DateTime today = DateTime.Today;
      int hour = int.Parse(regexMatch.Groups["hour"].Value);
      int minute = int.Parse(regexMatch.Groups["minute"].Value);
      DateTime newTime = new DateTime(today.Year, today.Month, today.Day, hour, minute, 0);
      return repository.GetStatus()
        .Bind(s => repository.SaveTodaySheet(s.Day.FixStartTime(newTime)))
        .Bind(_ => todaysSheet.ExecuteWithHeader());
    }
  }
}