namespace Kritikos.Configuration.Transformer.Tests
{
  using System.Collections.Generic;
  using System.IO;
  using System.Text;

  using Microsoft.Extensions.Logging.Abstractions;

  using Xunit;

  public class XmlDeletionTests
  {
    private const string Xml = @"<configuration>
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
        "accs-ac.appSettings.CBS_GetLoanPaymentDetailsUrl",
        "accs-ac.appSettings.CBS_LoanPaymentExecutionTimeout",
        "accs-ac.appSettings.CBS_LoanPaymentExecutionUrl",
        "accs-ac.appSettings.LISAExtensionChecks",
        "accs-ac.appSettings.PrepareLineForInterbank",
        "accs-ac.appSettings.PrepareLineForOrdered",
      };

      //var foo = ConfigurationHandlers.DeleteXmlKeys(NullLogger.Instance, stream, config, "DELETE_", true);
    }
  }
}
