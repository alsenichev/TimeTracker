using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Domain.Models;

namespace DataAccess.Mappers
{
  static class StatusMapper
  {
    public static Status Map(DayRecord record, TimeSpan stash)
    {
      return new Status(DayMapper.Map(record), stash);
    }
  }
}
