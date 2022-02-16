namespace MorePracticeMalodyServer.Model.DataModel;

/// <summary>
///     The Info of download files.
/// </summary>
public class DownloadInfo
{
    /// <summary>
    ///     File Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     File Hashs. Used to check integrity.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    ///     File download url.
    /// </summary>
    public string File { get; set; }
}