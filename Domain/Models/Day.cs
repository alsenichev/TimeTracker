using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Models
{
  public struct Day
  {
    public Day(DateTime dayStarted, TimeSpan @break, IList<TaskEntry> tasks)
    {
      DayStarted = dayStarted;
      Break = @break;
      Tasks = tasks;
    }

    public DateTime DayStarted { get; }

    public TimeSpan Break { get; }

    public IList<TaskEntry> Tasks { get; }

    public Day AddTask(TaskEntry task)
    {
      Tasks.Add(task);
      return new Day(DayStarted, Break, Tasks);
    }

    public Day AddToPause(TimeSpan pause)
    {
      return new Day(DayStarted, Break + pause, Tasks);
    }

    public Day AddToTaskDuration(int i, TimeSpan duration)
    {
      Tasks[i] = new TaskEntry(Tasks[i].Name, Tasks[i].Duration + duration);
      return new Day(DayStarted, Break, Tasks);
    }
  }
}