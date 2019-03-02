using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
  public class LogBookData
  {
    public IList<DayRecord> Days { get; set; }

    public TimeSpan Stash { get; set; }
  }
}