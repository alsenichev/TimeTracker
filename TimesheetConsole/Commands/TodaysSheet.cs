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

    private static string FormatStatus(Status status, bool includeHeader)
    {
      string startedAt = GetTime.FormatStatus(status);
      string header = includeHeader ? $"{startedAt}{Environment.NewLine}{Environment.NewLine}" : "";
      if (status.Day.Tasks == null || status.Day.Tasks.Count == 0)
      {
        return $"{header}No tasks created today.{Environment.NewLine}" +
               $"Unregistered time {GetTime.FormatTimeSpan(status.UnregisteredTime)}.";
      }

      var formattedEntries = status.Day.Tasks.Select((e, i) =>
        $"{i + 1}. {e.Name}, duration: {GetTime.FormatTimeSpan(e.Duration)}");
      var entries = string.Join(Environment.NewLine, formattedEntries);
      string stashPostfix = status.Stash > TimeSpan.Zero
        ? $" Stashed time {GetTime.FormatTimeSpan(status.Stash)}."
        : string.Empty;
      return
        $"{header}Here's the list of your tasks:{Environment.NewLine}{entries}{Environment.NewLine}" +
        $"Totally {GetTime.FormatTimeSpan(status.RegisteredTime)}. Unregistered time {GetTime.FormatTimeSpan(status.UnregisteredTime)}.{stashPostfix}";
    }

    public TodaysSheet(
      string name, Regex regex, MainRepository repository) : base(
      name, regex)
    {
      this.repository = repository;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      // todo - a bit of a hack here. to make it correct we would need an Execute<T> signature
      bool displayHeader = regexMatch != null;
      Result<string> formatSheet(Status sheet) =>
        Results.Success(FormatStatus(sheet, displayHeader));

      return repository.GetStatus().Bind(formatSheet);
    }
  }
}