using System;

namespace TimesheetConsole
{
  class Program
  {
    public static string ProductOwner = "Aleksey";
    private static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine(HelpInfo());
      }
      else
      {
        try
        {
          Console.WriteLine(RunCommand(args));
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
      }
      Console.ReadLine();
    }

    private static string RunCommand(string[] args)
    {
      switch (args[0])
      {
        case "run":
          return Commands.StartWorkingDay.Execute(DateTime.Now);
        case "time":
          return Commands.GetTime.Execute(DateTime.Now);
        default :
          return $"Unknown command: {args[0]}";
      }
    }

    private static string HelpInfo()
    {
      return
        @"Daily Log Version: 1.0.0

Usage: nn <command> [args]

Available commands:
run                     Starts the working day if it is not already started.
time                    Tells how much time has passed since the working day
                        started and how much time is left until the 8h working
                        day end.
log                     Displays the today's log.
list [number of days]   Displays log entries for the last month or for the
                        [number of days] if specified.";
    }
  }
}