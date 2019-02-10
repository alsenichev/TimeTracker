using System;
using System.Collections.Generic;
using DataAccess;
using Domain.BusinessRules;
using Domain.Models;

namespace TimesheetConsole.Commands
{
  public static class AddTask
  {
    public static string Execute(string[] args)
    {
      string name = string.Join(" ", args);
      var repo = new MainRepository();
      var log = repo.LoadLatestLog();
      if (log == null || !TimeManagement.IsToday(log.DayStarted))
      {
        StartWorkingDay.Execute(DateTime.Now);
        log = repo.LoadLatestLog();
      }
      var tasks = log.TaskEntries ?? new List<TaskEntry>();
      tasks.Add(new TaskEntry{ Name = name });
      log.TaskEntries = tasks;
      repo.UpdateTodaysLog(log);
      return TodaysLog.Execute(DateTime.Now, includeHeader:true);
    }
  }
}