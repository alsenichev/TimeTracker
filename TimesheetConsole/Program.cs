using System;
using System.Linq;

namespace TimesheetConsole
{
  class Program
  {
    public static string ProductOwner = "Aleksey";

    private static string HelpInfo()
    {
      return
@"Daily Log. Version: 1.0.0

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

    private static (string msg, bool exit) RunCommand(string[] args)
    {
      switch (args[0])
      {
        case "help":
        case "/?":
          return (HelpInfo(), false);
        case "run":
          return (Commands.StartWorkingDay.Execute(DateTime.Now), false);
        case "time":
          return (Commands.GetTime.Execute(DateTime.Now), false);
        case "task":
          EditTaskEntries(Commands.AddTask.Execute(args.Skip(1).ToArray()));
          return ("", true);
        case "log":
          EditTaskEntries(Commands.TodaysLog.Execute(DateTime.Now, includeHeader:true));
          return ("", true);
        default :
          return ($"Unknown command: {args[0]}", false);
      }
    }

    private static void EditTaskEntries(string message)
    {
      Console.WriteLine(message);
      for (;;)
      {
        Console.WriteLine(
                                          @"
                                          <i> x(.5)   to edit duration
                                          del <i>     to delete the task entry.
                                          add [task]  to add a new task entry.
                                          exit        to exit."
          );
        var input = Console.ReadLine();
        (bool success, bool exit) = Commands.UpdateTaskEntry.Execute(input.Trim());
        if (success)
        {
          Console.WriteLine();
          Console.WriteLine(Commands.TodaysLog.Execute(DateTime.Now, includeHeader:false));
          if (exit)
          {
            break;
          }
        }
        else
        {
          Console.WriteLine($"Unknown command: {input}");
        }
      }
    }

    private static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine(HelpInfo());
        Console.ReadLine();
      }
      else
      {
        try
        {
          (string msg, bool exit) = RunCommand(args);
          if (!exit)
          {
            Console.WriteLine(msg);
            Console.ReadLine();
          }
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
          Console.ReadLine();
        }
      }
    }
  }
}