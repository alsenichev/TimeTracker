using System;
using System.Collections.Generic;

namespace Domain.Models
{
  public class DailySheet
  {
    public DateTime DayStarted { get; set; }

    public TimeSpan Break { get; set; }

    public IList<TaskEntry> TaskEntries { get; set; }
  }
}