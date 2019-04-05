using System;
using System.Collections.Generic;

namespace Domain.Models
{
  public struct Day
  {
    public Day(DateTime dayStarted, TimeSpan @break, TimeSpan deposit, IList<TaskEntry> tasks)
    {
      DayStarted = dayStarted;
      Break = @break;
      Deposit = deposit;
      Tasks = tasks;
    }

    public DateTime DayStarted { get; }

    public TimeSpan Break { get; }

    public TimeSpan Deposit { get; }

    public IList<TaskEntry> Tasks { get; }

    public Day AddTask(TaskEntry task)
    {
      Tasks.Add(task);
      return new Day(DayStarted, Break, Deposit, Tasks);
    }

    public Day AddToPause(TimeSpan pause)
    {
      TimeSpan result = Break + pause;
      if (result < TimeSpan.Zero)
      {
        result = TimeSpan.Zero;
      }
      return new Day(DayStarted, result, Deposit, Tasks);
    }

    public Day AddToTaskDuration(int i, TimeSpan duration)
    {
      Tasks[i] = new TaskEntry(Tasks[i].Name, Tasks[i].Duration + duration);
      return new Day(DayStarted, Break, Deposit, Tasks);
    }
  }
}