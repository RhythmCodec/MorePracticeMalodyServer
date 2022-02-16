using MorePracticeMalodyServer.Model;

namespace MorePracticeMalodyServer.StorageProvider;

public class FileSystemStorageProvider : IStorageProvider
{
    public SignResponse Sign(int uid, int sid, int cid, string name, string hash, string host)
    {
        var resp = new SignResponse();

        // We just check if name and hash has the same count.
        var names = name.Split(',');
        var hashes = hash.Split(',');
        // If don't match, return with error.
        if (names.Length != hashes.Length)
        {
            resp.Code = 0;
            resp.ErrorIndex = names.Length - 1;
            resp.ErrorMsg = $"{names.Length} file(s) provide. But only {hashes.Length} hash(es) give to server.";

            return resp;
        }

        // Now we can upload.
        resp.Code = 0;
        resp.ErrorMsg = "";
        // Malody seems send one file a time.
        // Each file should have a meta Item, otherwise game will think something wrong.
        foreach (var h in hashes)
            resp.Meta.Add(new
            {
                Sid = sid,
                Cid = cid,
                Hash = h
            });

        resp.Host = $"http://{host}/api/SelfUpload/receive";

        return resp;
    }
}