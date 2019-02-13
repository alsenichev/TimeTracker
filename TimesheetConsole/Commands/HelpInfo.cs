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
run                     Starts the working day if it is not already started.
time                    Tells how much time has passed since the working day
                        started and how much time is left until the 8h working
                        day end.
log                     Displays the today's sheet.
list [number of days]   Displays sheets for the last month or for the
                        [number of days] if specified
<i> <hours[.5]>         set duration of the task at index <i> in today's sheet
del <i>                 delete the task at index <i> in today's sheet
add <task>              add a new task entry.
exit                    exit the program";
      return Results.Success(info);
    }
  }
}