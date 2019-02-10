using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class DailyLog
    {
      public DateTime DayStarted { get; set; }
      public IList<TaskEntry> TaskEntries { get; set; }
    }
}
