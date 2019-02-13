using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataAccess;
using Domain.BusinessRules;
using Domain.Models;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public class AddTask:AppCommandBase
  {
    private readonly MainRepository repository;
    private readonly StartWorkingDay startWorkingDay;
    private readonly TodaysSheet todaysSheet;

    public AddTask(
      string name,
      Regex regex,
      MainRepository repository,
      StartWorkingDay startWorkingDay,
      TodaysSheet todaysSheet) : base(name, regex)
    {
      this.repository = repository;
      this.startWorkingDay = startWorkingDay;
      this.todaysSheet = todaysSheet;
    }

    public override Result<string> Execute(Match regexMatch)
    {
      Result<object> createTask(DailySheet sheet)
      {
        var tasks = sheet.TaskEntries ?? new List<TaskEntry>();
        tasks.Add(new TaskEntry{ Name = regexMatch.Groups["entry"].Value });
        sheet.TaskEntries = tasks;
        return repository.SaveTodaySheet(sheet);
      }

      Result<object> startAndCreateTask() =>
        startWorkingDay.Execute(null)
          .Bind(_ => repository.GetTodaySheet())
          .Bind(o => o.Fold(
            createTask,
            () => Results.Failure<object>("Failed to create a new task.")));

      return repository.GetTodaySheet()
        .Bind(o => o.Fold(createTask, startAndCreateTask)).Bind(_ => todaysSheet.Execute(null));
    }
  }
}