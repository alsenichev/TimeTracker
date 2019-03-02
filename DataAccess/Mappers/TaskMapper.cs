using DataAccess.Models;
using Domain.Models;

namespace DataAccess.Mappers
{
  static class TaskMapper
  {
    public static TaskEntry Map(TaskRecord record)
    {
      return new TaskEntry(record.Name, record.Duration);
    }

    public static TaskRecord Map(TaskEntry taskEntry)
    {
      return new TaskRecord {Duration = taskEntry.Duration, Name = taskEntry.Name};
    }
  }
}