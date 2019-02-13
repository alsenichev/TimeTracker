using System.Text.RegularExpressions;
using Domain.Utils;

namespace TimesheetConsole.Commands
{
  public interface IAppCommand
  {
    Regex CommandRegex { get; }

    string Name { get; }

    Result<string> Execute(Match regexMatch);
  }

  public abstract class AppCommandBase : IAppCommand
  {
    public AppCommandBase(string name, Regex regex)
    {
      Name = name;
      CommandRegex = regex;
    }

    public Regex CommandRegex { get; }

    public string Name { get; set; }

    //Todo not a Match but a full-blown parameter object.
    public abstract Result<string> Execute(Match regexMatch);
  }
}