using System.Collections.Generic;

namespace MorePracticeMalodyServer.Model;

/// <summary>
///     Warp response to the specific response structure.
/// </summary>
/// <typeparam name="T">The data model of <see cref="Data" />.</typeparam>
public class Response<T>
{
    /// <summary>
    ///     Response state. Return 0 for all success response.
    /// </summary>
    public int Code { get; set; } = 0;

    /// <summary>
    ///     If this <see cref="Data" /> has more data to show, set this to true.
    /// </summary>
    public bool HasMore { get; set; } = false;

    /// <summary>
    ///     If has more data, set this to the next page index.
    /// </summary>
    public int Next { get; set; } = 0;

    /// <summary>
    ///     The data that client requested.
    /// </summary>
    public List<T> Data { get; set; } = new();
}