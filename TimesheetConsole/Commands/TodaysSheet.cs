using System;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class TodaysSheet : AppCommandBase
  {
    private readonly MainRepository repository;

    #region private methods

    private static string FormatStatus(Status status, bool includeHeader)
    {
      string header = includeHeader
        ? $"{MoreToWorkOrOvertime(status)}{Environment.NewLine}{Environment.NewLine}"
        : "";
      if (status.Day.Tasks == null || status.Day.Tasks.Count == 0)
      {
        return $"{header}No tasks created today.{Environment.NewLine}" +
               $"Unregistered time {FormatTimeSpan(status.UnregisteredTime)}.";
      }
      var formattedEntries = status.Day.Tasks.Select((e, i) =>
        $"{i + 1}. {e.Name}, duration: {FormatTimeSpan(e.Duration)}");
      var entries = string.Join(Environment.NewLine, formattedEntries);
      string stashPostfix = status.Stash > TimeSpan.Zero
        ? $" Stashed time {FormatTimeSpan(status.Stash)}."
        : string.Empty;
      return
        $"{header}Here's the list of your tasks:{Environment.NewLine}{entries}{Environment.NewLine}" +
        $"Unregistered time {FormatTimeSpan(status.UnregisteredTime)}.{stashPostfix} Totally {FormatTimeSpan(status.RegisteredTime)}.";
    }

    private static string MoreToWorkOrOvertime(Status status)
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

    private static string MoreToWork(Status status)
    {
      return $"You've started today at {status.StartedAt:t}{PausePostfix(status)}" +
             $".{Environment.NewLine}Your working day ends at {status.EndOfDay:t} in {FormatTimeSpan(status.Left)}.";
    }

    private static string PausePostfix(Status status)
    {
      string pausePostfix =
        status.Pause == TimeSpan.Zero
          ? string.Empty
          : status.Pause > TimeSpan.Zero
            ? $" and took a {FormatTimeSpan(status.Pause)} pause"
            : $" and have {FormatTimeSpan(-status.Pause)} deposed";
      return pausePostfix;
    }

    private static string Overtime(Status status)
    {
      return $"You've started today at {status.StartedAt:t}{PausePostfix(status)}" +
             $".{Environment.NewLine}It makes a {FormatTimeSpan(status.Overtime)} overtime.";
    }

    private Result<string> Execute(bool displayHeader)
    {
      Result<string> formatSheet(Status sheet) =>
        Results.Success(FormatStatus(sheet, displayHeader));
      return repository.GetStatus().Bind(formatSheet);
    }

    #endregion

    #region public methods

    public TodaysSheet(
      string name, Regex regex, MainRepository repository) : base(
      name, regex)
    {
      this.repository = repository;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      return ExecuteWithHeader();
    }

    public Result<string> ExecuteWithNoHeader()
    {
      return Execute(false);
    }

    public Result<string> ExecuteWithHeader()
    {
      return Execute(true);
    }

    public static string FormatTimeSpan(TimeSpan time)
    {
      return $"{time.Hours}h {time.Minutes}m";
    }

    #endregion
  }
}