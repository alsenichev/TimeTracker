using System.Collections.Generic;
using Domain;
using Domain.Models;

namespace DataAccess
{
  public class TimesheetData
  {
    public IList<DailySheet> Logs { get; set; }
  }
}