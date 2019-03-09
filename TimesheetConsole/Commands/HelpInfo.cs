using System.Text.RegularExpressions;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  class HelpInfo:AppCommandBase
  {
    public HelpInfo(string name, Regex regex) : base(name, regex){}

    public override Result<string> Execute(Match regexMatch)
    {
      const string info=
        @"Daily Log. Version: 1.0.0

Usage: <command> [args]

Available commands:
log                     Displays today's sheet.
pause [-]<m>            Adds <m> minutes to the paused time
                        (subtracts if negative).
<i> <hours[.5]>         Sets the duration of a task at index <i> in today's
                        sheet. <hours> is an integer, [.5] is the half hour.
del <i>                 Deletes a task at index <i> in today's sheet.
add <task>              Adds a new task entry.
stash                   Stashes unregistered time.
expend                  Gets back unregistered time from stash.
list [number of days]   Displays sheets for the last month or for the
                        [number of days] if specified.
exit                    Exits the program.";
      return Results.Success(info);
    }
  }
}