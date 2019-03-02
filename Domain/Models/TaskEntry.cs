using System;

namespace Domain.Models
{
  public struct TaskEntry
  {
    public TaskEntry(string name, TimeSpan duration)
    {
      Name = name;
      Duration = duration;
    }

    public string Name { get; }

    public TimeSpan Duration { get; }
  }
}