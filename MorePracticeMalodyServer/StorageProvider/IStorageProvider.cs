using MorePracticeMalodyServer.Model;

namespace MorePracticeMalodyServer.StorageProvider;

public interface IStorageProvider
{
    /// <summary>
    ///     Get sign for upload phase 1. See
    ///     <a href="https://gitlab.com/mugzone_team/malody_store_api#chart-upload">Document</a> for more info.<br />
    /// </summary>
    /// <param name="uid">User id</param>
    /// <param name="sid">Uploaded song id</param>
    /// <param name="cid">Uploaded chart id</param>
    /// <param name="name">file name set, connected by commas</param>
    /// <param name="hash">file md5 set, connected by commas</param>
    /// <param name="host">Server hostname.</param>
    /// <returns>Response of Sign.</returns>
    public SignResponse Sign(int uid, int sid, int cid, string name, string hash, string host);

    public 
}