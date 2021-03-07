namespace Kritikos.Configuration.Transformer
{
  public static class LogTemplates
  {
    // Critical
    public const string UnhandledException = "Unhandled exception: {Message}";
    public const string FileNotFound = "Could not find file {File}";
    public const string FileIsReadOnly = "Could not open file {File} for writing";

    // Error
    public const string MissingNode = "Could not target node identified by key {Node}";

    public const string UnsupportedNode =
      "Node {Node} not supported, only appSettings, connectionStrings and EndPoints are supported!";

    // Warning

    // Information
    public const string DeletingNode = "Removing node {Node}";
    // Debug
    public const string OpeningFileForWrite = "Opening file {File} for write";
    public const string LocatingNode = "Locating node {Node}";
  }
}
