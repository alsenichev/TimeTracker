using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.Utils;
using TimesheetConsole.Commands;

namespace TimesheetConsole
{
  class Program
  {
    public static string ProductOwner = "Aleksey";

    private static readonly Regex help = new Regex(@"^\s*(help|\/\?)\s*$");
    private static readonly Regex run = new Regex(@"^\s*run\s*$");
    private static readonly Regex exit = new Regex(@"(^\s*$|^\s*exit\s*$)");
    private static readonly Regex time = new Regex(@"^\s*time\s*$");
    private static readonly Regex log = new Regex(@"^\s*log\s*$");
    private static readonly Regex deleteTask = new Regex(@"^\s*del\s+(?<index>\d{1,3})\.?\s*$");
    private static readonly Regex setDuration = new Regex(@"^(?<index>\d{1,3})\.?\s*(?<hours>\d{1,3})(?<fraction>\.5)?$");
    private static readonly Regex addTask = new Regex(@"^\s*add\s*(?<entry>.*$)");
    private static readonly Regex list = new Regex(@"^\s*list\s*(?<count>\s\d{1,3})?\s*$");

    private static readonly MainRepository repository = new MainRepository();

    private static readonly HelpInfo helpCommand = new HelpInfo("Help", help);
    private static readonly StartWorkingDay startDayCommand =
      new StartWorkingDay("Start working day command", run, repository);
    private static readonly GetTime getTimeCommand =
      new GetTime("Get time command", time, repository);
    private static readonly TodaysSheet todaysSheetCommand =
      new TodaysSheet("Today's sheet command", log, repository, startDayCommand);
    private static readonly AddTask addTaskCommand =
      new AddTask("Add task command", addTask,repository, startDayCommand, todaysSheetCommand);
    private static readonly DeleteTask deleteTaskCommand =
      new DeleteTask("Delete task command", deleteTask, repository, todaysSheetCommand);
    private static readonly SetTaskDuration setTaskDurationCommand =
      new SetTaskDuration("Set task duration command", setDuration, repository, todaysSheetCommand);
    private static readonly ListSheets listSheetsCommand =
      new ListSheets("List sheets command", list, repository);

    private static readonly IList<IAppCommand> allCommands = new List<IAppCommand>
    {
      helpCommand,
      startDayCommand,
      getTimeCommand,
      todaysSheetCommand,
      addTaskCommand,
      deleteTaskCommand,
      setTaskDurationCommand,
      listSheetsCommand
    };

    private static void DisplayResult(Result<string> commandResult)
    {
      if (!commandResult.IsSuccess)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Join(Environment.NewLine, commandResult.Messages));
        Console.ForegroundColor = ConsoleColor.White;
      }
      else
      {
        Console.WriteLine(commandResult.Value);
      }
    }

    private static Result<string> ExecuteCommand(string input)
    {
      var matches = allCommands.Select(c =>
      new {Match = c.CommandRegex.Match(input), Command = c}).Where(m => m.Match.Success).ToList();
      switch (matches.Count)
      {
          case 0:
            return Results.Failure<string>($"Unrecognized command: {input}");
          case 1:
            return matches[0].Command.Execute(matches[0].Match);
          default:
            return Results.Failure<string>(
              $"'{input}' matches more than one command: {string.Join(", ", matches.Select(m=>m.Command.Name))}");
      }
    }

    private static void Main(string[] args)
    {
      DisplayResult(ExecuteCommand(string.Join(" ",args)));
      for (;;)
      {
        Console.WriteLine();
        var input = Console.ReadLine();
        if (exit.IsMatch(input??string.Empty))
        {
          break;
        }
        DisplayResult(ExecuteCommand(input));
      }
    }
  }
}