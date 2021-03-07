namespace Kritikos.Configuration.Transformer
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public enum XmlNodeMode
  {
    Unsupported,
    ConnectionString,
    AppSetting,
    Endpoint,
  }
}
