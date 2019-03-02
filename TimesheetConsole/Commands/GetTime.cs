using System;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class GetTime : AppCommandBase
  {
    private readonly MainRepository repository;

    public static string FormatTimeSpan(TimeSpan time)
    {
      return $"{time.Hours}h {time.Minutes}m";
    }

    private static string MoreToWork(Status status)
    {
      string pausePostfix =
        status.Pause == TimeSpan.Zero
          ? string.Empty
          : status.Pause > TimeSpan.Zero
            ? $" and took a {FormatTimeSpan(status.Pause)} pause"
            : $" and have {FormatTimeSpan(-status.Pause)} deposed";
      return $@"You've started today at {status.StartedAt:t}{pausePostfix}.{Environment.NewLine}Your working day ends at {status.EndOfDay:t} in {FormatTimeSpan(status.Left)}.";
    }

    private static string Overtime(Status status)
    {
      string pausePostfix =
        status.Pause.Equals(TimeSpan.Zero) ? string.Empty : $" and took a {FormatTimeSpan(status.Pause)} pause";
      return
        $"You've started today at {status.StartedAt:t}{pausePostfix}.{Environment.NewLine}It makes a {FormatTimeSpan(status.Overtime)} overtime.";
    }

    #region public methods

    public static string FormatStatus(Status status)
    {
      if (status.IsOvertime)
      {
        return Overtime(status);
      }
      else
      {
        return MoreToWork(status);
      }
    }

    public GetTime(string name, Regex regex, MainRepository repository) : base(name, regex)
    {
      this.repository = repository;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      return repository.GetStatus()
        .Map(FormatStatus);
    }
  }
    #endregion
}