using System;

namespace Domain.Models
{
  public class TaskEntry
  {
    public string Name { get; set; }

    public TimeSpan Duration { get; set; }
  }
}