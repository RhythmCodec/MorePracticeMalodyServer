using Microsoft.EntityFrameworkCore;

namespace MorePracticeMalodyServer.Model.DbModel;

/// <summary>
///     Promoted song list
/// </summary>
[Index(nameof(Id), nameof(Song), IsUnique = true)]
public class Promotion
{
    public int Id { get; set; }

    /// <summary>
    ///     Promoted song.
    /// </summary>
    public Song? Song { get; set; }
}