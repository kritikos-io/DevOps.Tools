namespace Kritikos.Configuration.Transformer
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;

  public static class XmlExtensions
  {
    public static XmlNodeMode ParseMode(this string key) => key switch
    {
      var s when s.Contains("connectionStrings", StringComparison.CurrentCultureIgnoreCase) => XmlNodeMode
        .ConnectionString,
      var s when s.Contains("appSettings", StringComparison.InvariantCultureIgnoreCase) => XmlNodeMode.AppSetting,
      var s when s.Contains("client", StringComparison.InvariantCultureIgnoreCase) => XmlNodeMode.Endpoint,
      _ => XmlNodeMode.Unsupported,
    };

    public static readonly IReadOnlyDictionary<XmlNodeMode, string> IdentifierAttribute =
      new Dictionary<XmlNodeMode, string>
      {
        { XmlNodeMode.AppSetting, "key" }, { XmlNodeMode.ConnectionString, "name" }, { XmlNodeMode.Endpoint, "name" },
      };

    public static readonly IReadOnlyDictionary<XmlNodeMode, string> ValueAttribute =
      new Dictionary<XmlNodeMode, string>
      {
        { XmlNodeMode.AppSetting, "value" },
        { XmlNodeMode.ConnectionString, "connectionString" },
        { XmlNodeMode.Endpoint, "address" },
      };

    public static XElement? LocateElement(
      this XDocument doc,
      string[] pathSegments,
      XmlNodeMode mode,
      bool isCaseInsensitive)
    {
      var element =
        doc.Elements()
          .SingleOrDefault(x => x.Name.LocalName.ToSelectedCasing(isCaseInsensitive) == pathSegments.First());
      if (element == null)
      {
        return element;
      }

      var segments = pathSegments.Skip(1).Take(pathSegments.Length - 2).ToList();
      while (segments.Any())
      {
        element = element?.Elements()
          .SingleOrDefault(x => x.Name.LocalName.ToSelectedCasing(isCaseInsensitive) == segments.First());
        if (element == null)
        {
          break;
        }

        segments.Remove(segments.First());
      }

      element = element?.Elements()
        .SingleOrDefault(x =>
          x.HasAttributes &&
          x.Attributes()
            .Any(y => y.Name.LocalName.ToSelectedCasing(isCaseInsensitive)
                      == IdentifierAttribute[mode].ToSelectedCasing(isCaseInsensitive)
                      && y.Value.ToSelectedCasing(isCaseInsensitive) == pathSegments.Last()));

      return element;
    }

    public static string ToSelectedCasing(this string s, bool isCaseInsensitive)
      =>
        isCaseInsensitive
          ? s.ToLowerInvariant()
          : s;
  }
}
