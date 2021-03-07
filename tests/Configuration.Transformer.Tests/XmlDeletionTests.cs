namespace Kritikos.Configuration.Transformer.Tests
{
  using System.Collections.Generic;
  using System.Data.Common;
  using System.IO;
  using System.Text;
  using System.Xml.Linq;

  using Microsoft.Extensions.Logging.Abstractions;

  using Xunit;

  public class XmlDeletionTests
  {
    private const string Xml = @"<configuration>
                                    <appSettings>
                                      <add key=""Location"" value=""Home"" />
                                    </appSettings>
                                    <location path=""forum"">
                                    <system.web>
                                      <customErrors mode=""RemoteOnly"" defaultRedirect=""forum-error.aspx"">
                                        <error statusCode=""404"" redirect=""forum-file-not-found.aspx"" />
                                      </customErrors>
                                    </system.web>
                                    </location>
                                </configuration>";

    [Fact]
    public void Delete_Prefixed_Keys()
    {
      using MemoryStream stream = new();
      using StreamWriter writer = new(stream);
      writer.Write(Xml);
      writer.Flush();
      stream.Position = 0;

      var config = new List<string>
      {
        "configuration.appSettings.location",
      };

      var removed = ConfigurationHandlers.DeleteXmlKeys(NullLogger.Instance, stream, config, true);

      var xdoc = XDocument.Parse(removed);
      var element = xdoc.LocateElement(new[] { "configuration", "appSettings", "location" }, XmlNodeMode.AppSetting, true);

      Assert.Null(element);
    }
  }
}
