using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
  public class DayRecord
  {
    public DateTime DayStarted { get; set; }

    public TimeSpan Break { get; set; }

    public TimeSpan Deposit { get; set; }

    public IList<TaskRecord> Tasks { get; set; }
  }
}
