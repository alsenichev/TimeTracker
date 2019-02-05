using System.Collections.Generic;
using Domain;

namespace DataAccess
{
  public class TimesheetData
  {
    public IList<DailyLog> Logs { get; set; }
  }
}