namespace Kritikos.Configuration.Transformer.Commands
{
  using System.CommandLine;
  using System.CommandLine.Invocation;
  using System.IO;

  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  public class XmlDeletionCommand : Command
  {
    private static readonly Option<FileInfo> InputOption = new(new[] { "--input", "-i" })
    {
      Name = "input", Description = "The xml file to operate on", IsRequired = true,
    };

    private static readonly Option<string> PrefixOption = new(new[] { "--prefix", "-p" })
    {
      Name = "prefix", Description = "The prefix that will be used to choose which keys to delete", IsRequired = true,
    };

    public XmlDeletionCommand()
      : base("deleteKeys",
        "Loads environment variables with selected prefix and removes them from xml formatted files. The value of each variable should be the file name.")
    {
      AddOption(InputOption);
      AddOption(PrefixOption);
      AddOption(TransformerCommandExtensions.CasingOption);
      Handler = CommandHandler.Create(
        (IHost host, FileInfo input, string prefix, bool dry, InvocationContext ctx) =>
          ConfigurationHandlers.DeleteKeysFromXmlCommand(
            host.Services.GetRequiredService<ILogger>(),
            input,
            prefix,
            ctx.ParseOption<bool>(TransformerCommandExtensions.CasingOption),
            ctx.ParseOption<bool>(TransformerCommandExtensions.DryOption)));
    }
  }
}
