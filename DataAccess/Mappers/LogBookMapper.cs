using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using Domain.Models;

namespace DataAccess.Mappers
{
  static class LogBookMapper
  {
    public static LogBook SafeMap(LogBookData data)
    {
      return new LogBook(
        data?.Days?.Select(d => DayMapper.Map(d)).ToList() ?? new List<Day>(),
        data?.Stash?? TimeSpan.Zero);
    }
  }
}