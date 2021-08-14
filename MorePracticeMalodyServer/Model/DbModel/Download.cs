using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MorePracticeMalodyServer.Model.DbModel
{
    /// <summary>
    ///     Main data types used for download.
    /// </summary>
    [Index("Chart")]
    public class Download // Unknown types are begin with '¿'.
    {
        /// <summary>
        ///     ¿Chart Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     ¿Downloading Chart Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Downloading File Hash. Ched to ck integrityity.
        /// </summary>
        [MaxLength(32)]
        public string Hash { get; set; }

        /// <summary>
        ///     File url.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        ///     Navigation to <see cref="DbModel.Chart" />.
        /// </summary>
        public Chart Chart { get; set; }
    }
}