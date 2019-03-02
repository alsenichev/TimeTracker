using System;

namespace DataAccess.Models
{
  public struct TaskRecord
  {
    public string Name { get; set; }

    public TimeSpan Duration { get; set; }
  }
}