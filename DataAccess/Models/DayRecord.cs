using System;
using System.Collections.Generic;
using Domain.Models;

namespace DataAccess.Models
{
  public class DayRecord
  {

    public DateTime DayStarted { get; set; }

    public TimeSpan Break { get; set; }

    public IList<TaskRecord> Tasks { get; set; }
  }
}