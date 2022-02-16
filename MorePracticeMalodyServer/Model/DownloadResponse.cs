using System.Collections.Concurrent;
using MorePracticeMalodyServer.Model.DataModel;

namespace MorePracticeMalodyServer.Model;

/// <summary>
///     The data response at downloading.
/// </summary>
public class DownloadResponse
{
    /// <summary>
    ///     Status response Code.
    /// </summary>
    public int Code { get; set; } = 0;

    /// <summary>
    ///     Chart Songs id.
    /// </summary>
    public int Sid { get; set; } = 0;

    /// <summary>
    ///     Chart id.
    /// </summary>
    public int Cid { get; set; } = 0;

    /// <summary>
    ///     Chart File Info.
    /// </summary>
    public ConcurrentBag<DownloadInfo> Items { get; set; } = new();
}