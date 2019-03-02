using System;
using System.Collections.Generic;

namespace Domain.Models
{
  public class LogBook
  {
    public LogBook(IList<Day> days, TimeSpan stash)
    {
      Days = days;
      Stash = stash;
    }

    public IList<Day> Days { get; }

    public TimeSpan Stash { get; }
  }
}