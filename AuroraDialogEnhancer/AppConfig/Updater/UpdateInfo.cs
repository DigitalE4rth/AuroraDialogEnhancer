using System;
using System.Xml.Serialization;

namespace AuroraDialogEnhancer.AppConfig.Updater;

[XmlRoot("item")]
public class UpdateInfo : EventArgs
{
    public bool IsUpdateAvailable { get; set; }

    [XmlIgnore]
    public Exception? Error { get; set; }

    [XmlElement("uri")]
    public string DownloadUri { get; set; } = string.Empty;

    [XmlElement("changelog")]
    public string ChangelogUri { get; set; } = string.Empty;

    [XmlElement("version")]
    public string Version { get; set; } = string.Empty;

    public Version InstalledVersion { get; set; } = new();

    /// <summary>
    /// Executable path of the updated application relative to installation directory.
    /// </summary>
    [XmlElement("executable")]
    public string ExecutablePath { get; set; } = string.Empty;

    /// <summary>
    /// Command line arguments used by Installer.
    /// </summary>
    [XmlElement("args")]
    public string InstallerArgs { get; set; } = string.Empty;
}
