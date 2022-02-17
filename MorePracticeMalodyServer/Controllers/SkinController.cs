using Microsoft.AspNetCore.Mvc;
using MorePracticeMalodyServer.Model;
using MorePracticeMalodyServer.Model.DataModel;

namespace MorePracticeMalodyServer.Controllers;

/// <summary>
///     Provides skin list.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SkinController : ControllerBase
{
    // Now we don't support skin list.
    [Route("list")]
    [HttpGet]
    public async Task<Response<SkinInfo>> GetSkinList(Platform plat, Mode mode, string word, int from, int v)
    {
        return new Response<SkinInfo>();
    }

    // Now we don't support skin download.
    [Route("buy")]
    [HttpGet]
    public async Task<object> GetSkinDownloadUrl(int sid)
    {
        return new
        {
            Code = 0,
            Data = new { }
        };
    }
}