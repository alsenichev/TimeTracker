using System;
using DataAccess.Models;
using Domain.Models;

namespace DataAccess.Mappers
{
  internal static class StatusMapper
  {
    public static Status Map(DayRecord record, TimeSpan stash)
    {
      return new Status(DayMapper.Map(record), stash);
    }
  }
}
