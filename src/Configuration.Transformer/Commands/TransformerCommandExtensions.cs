namespace Kritikos.Configuration.Transformer.Commands
{
  using System.CommandLine;
  using System.CommandLine.Builder;
  using System.CommandLine.Invocation;
  using System.CommandLine.Parsing;
  using System.Linq;

  using Microsoft.Extensions.DependencyInjection;

  public static class TransformerCommandExtensions
  {
    public static readonly Option<bool> QuietOption =
      new(new[] { "--quiet", "-q" }) { IsRequired = false, Description = "Suppresses non critical messages", };

    public static readonly Option<bool> CasingOption =
      new(new[] { "--case-insensitive" })
      {
        Name = "case-insensitive", Description = "Ignore casing when comparing keys, for Azure DevOps environment variable exposure. WARNING, xml is supposed to be case sensitive, validate before using!", IsRequired = false,
      };

    public static readonly Option<bool> VerboseOption =
      new(new[] { "--verbose", "-v" })
      {
        IsRequired = false, Description = "Increases verbosity for debugging purposes",
      };

    public static readonly Option<bool> DryOption =
      new(new[] { "--dry", "-d" })
      {
        Description = "Simulate operation without touching files", IsRequired = false,
      };

    public static CommandLineBuilder RegisterCommands(this CommandLineBuilder builder)
    {
      var services = new ServiceCollection();

      var type = typeof(TransformerCommandExtensions);
      var commandType = typeof(Command);
      var commands = type.Assembly.GetExportedTypes()
        .Where(x => x.Namespace == type.Namespace && commandType.IsAssignableFrom(x))
        .ToList();

      foreach (var command in commands)
      {
        services.AddSingleton(commandType, command);
      }

      var provider = services.BuildServiceProvider();
      var cliCommands = provider.GetServices<Command>();
      foreach (var cli in cliCommands)
      {
        builder.AddCommand(cli);
      }

      return builder;
    }

    public static T? ParseOption<T>(this InvocationContext ctx, IOption option)
    {
      var result = ctx.ParseResult.FindResultFor(option);
      return result == null
        ? default
        : result.GetValueOrDefault<T>();
    }
  }
}
