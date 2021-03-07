namespace Kritikos.Configuration.Transformer
{
  using System;
  using System.CommandLine.Builder;
  using System.CommandLine.Hosting;
  using System.CommandLine.Parsing;
  using System.Threading.Tasks;

  using Kritikos.Configuration.Transformer.Commands;

  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  using Serilog;
  using Serilog.Core;
  using Serilog.Events;
  using Serilog.Extensions.Logging;
  using Serilog.Sinks.SystemConsole.Themes;

  public sealed class Program
  {
    private static readonly LoggingLevelSwitch LevelSwitch = new();

    public static async Task<int> Main(string[] args)
    {
      Log.Logger = CreateLoggerConfiguration()
        .CreateLogger();

      var parser = BuildCommandLine();
      var command = parser.Parse(args);
      var verbose = command.ValueForOption<bool>("--verbose");
      var quiet = command.ValueForOption<bool>("--quiet");

      LevelSwitch.MinimumLevel = verbose switch
      {
        true => LogEventLevel.Debug,
        _ when quiet => LogEventLevel.Fatal,
        _ => LogEventLevel.Information,
      };

      try
      {
        return await parser
          .InvokeAsync(args);
      }
      catch (Exception e)
      {
        Log.Logger.Fatal(e, LogTemplates.UnhandledException, e.Message);
      }
      finally
      {
        Log.CloseAndFlush();
      }

      return -1;
    }

    private static Parser BuildCommandLine()
      => new CommandLineBuilder()
        .UseHost(
          _ => Host.CreateDefaultBuilder(),
          host => host
            .UseSerilog()
            .ConfigureServices(sp =>
            {
              sp.AddSingleton<ILoggerFactory>(new SerilogLoggerFactory());
              sp.AddSingleton(sp =>
                sp.GetRequiredService<ILoggerFactory>().CreateLogger("Configuration.Transformer"));
            }))
        .AddGlobalOption(TransformerCommandExtensions.VerboseOption)
        .AddGlobalOption(TransformerCommandExtensions.QuietOption)
        .AddGlobalOption(TransformerCommandExtensions.DryOption)
        .UseDefaults()
        .RegisterCommands()
        .Build();

    private static LoggerConfiguration CreateLoggerConfiguration()
      => new LoggerConfiguration()
        .Filter.ByExcluding(
          "SourceContext='Microsoft.Hosting.Lifetime' or SourceContext='Microsoft.Extensions.Hosting.Internal.Host'")
        .MinimumLevel.ControlledBy(LevelSwitch)
        .WriteTo.Console(theme: AnsiConsoleTheme.Code);
  }
}
