using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using Domain.Models;

namespace DataAccess.Mappers
{
  static class DayMapper
  {
    public static Day Map(DayRecord record)
    {
      return new Day(
        record.DayStarted,
        record.Break,
        record.Tasks?.Select(t => TaskMapper.Map(t)).ToList() ?? new List<TaskEntry>());
    }

    public static DayRecord Map(Day day)
    {
      return new DayRecord
      {
        Break = day.Break,
        DayStarted = day.DayStarted,
        Tasks = day.Tasks.Select(t => TaskMapper.Map(t)).ToList()
      };
    }
  }
}