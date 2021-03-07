namespace Kritikos.Configuration.Transformer
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Xml.Linq;

  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Logging;

  public static class ConfigurationHandlers
  {
    public static string DeleteXmlKeys(
      ILogger logger,
      Stream input,
      ICollection<string> configuration,
      bool isCaseInsensitive)
    {
      using var reader = new StreamReader(input);
      var content = reader.ReadToEnd();

      var doc = XDocument.Parse(content, LoadOptions.None);

      foreach (var key in configuration)
      {
        logger.LogDebug(LogTemplates.LocatingNode, key);
        var split = key.Split(".").Select(x => x.ToSelectedCasing(isCaseInsensitive)).ToArray();

        var xmlNodeMode = key.ParseMode();

        if (xmlNodeMode == XmlNodeMode.Unsupported)
        {
          logger.LogError(LogTemplates.UnsupportedNode, key);
          continue;
        }

        var element = doc.LocateElement(split, xmlNodeMode, isCaseInsensitive);

        if (element is null)
        {
          logger.LogError(LogTemplates.MissingNode, key);
          continue;
        }

        logger.LogInformation(LogTemplates.DeletingNode, key);
        element.Remove();
      }

      return doc.ToString();
    }

    internal static async Task<int> DeleteKeysFromXmlCommand(
      ILogger logger,
      FileInfo input,
      string prefix,
      bool isCaseInsensitive,
      bool dryRun)
    {
      if (!input.Exists)
      {
        logger.LogCritical(LogTemplates.FileNotFound, input.FullName);
        return -1;
      }

      var config = new ConfigurationBuilder()
        .AddEnvironmentVariables(prefix)
        .Build()
        .AsEnumerable()
        .Where(x => x.Value == input.Name)
        .ToList();

      logger.LogDebug(LogTemplates.OpeningFileForWrite, input.FullName);
      await using var stream = input.OpenRead();
      var xml = DeleteXmlKeys(
        logger,
        stream,
        config.Select(x => x.Key).ToList(),
        isCaseInsensitive);
      try
      {
        await using var output = input.OpenWrite();
        await using StreamWriter writer = new(output);
        if (!dryRun)
        {
          await writer.WriteAsync(xml);
          await writer.FlushAsync();
        }

        return 1;
      }
      catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
      {
        logger.LogCritical(ex, LogTemplates.FileIsReadOnly, input.FullName);
      }

      return -1;
    }
  }
}
