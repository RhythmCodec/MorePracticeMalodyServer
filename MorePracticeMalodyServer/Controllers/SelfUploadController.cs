/* Copyright (C) 2021 RhythmCodec
 * See @RhythmCodec at https://github.com/RhythmCodec
 * 
 * Last Update: 2021/08/22
 * Author: Kami11
 * Last Modifier: soloopooo
 * Description:
 *      
 *      Upload Controller used by LOCAL(Server based) server
 * 
 * Providing datas:
 *      
 *      None.
 *     
 *
 * Known bugs: 
 * 
 *      
 *      
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace MorePracticeMalodyServer.Controllers;

/// <summary>
///     Uses to process files when configured to self provide mode.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SelfUploadController : ControllerBase
{
    [HttpPost]
    [Route("receive")]
    public async Task<IActionResult> ReceiveFiles([FromForm] int sid, [FromForm] int cid, [FromForm] string hash)
    {
        // Make sure target directory exist.
        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "wwwroot", sid.ToString(),
                cid.ToString())))
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "wwwroot", sid.ToString(),
                cid.ToString()));

        var files = Request.Form.Files;
        using var md5 = MD5.Create();
        foreach (var formFile in files) // Save files.
        {
            // Check file first.
            // Malody upload one file each time, with hash in Meta[index], so we just check once.
            string checksum;
            if (formFile.FileName.Contains(".mc")) // Mc file is compressed, so we should decompress it and check.
            {
                using var zipFile = new ZipArchive(formFile.OpenReadStream());
                var entry = zipFile.GetEntry(formFile.FileName);
                await using var decompressed = entry.Open();

                var md5byte = await md5.ComputeHashAsync(decompressed);
                checksum = BitConverter.ToString(md5byte).Replace("-", "").ToLower();
            }
            else // check other files directly.
            {
                var md5byte = await md5.ComputeHashAsync(formFile.OpenReadStream());
                checksum = BitConverter.ToString(md5byte).Replace("-", "").ToLower();
            }

            if (checksum != hash)
                return Conflict(); // Just give some error to stop upload.

            // Write file to our local filesystem.
            await using var fs = System.IO.File.Open(
                Path.Combine(Environment.CurrentDirectory, "wwwroot", sid.ToString(), cid.ToString(),
                    // FIX issue #1, encode our file name, make it safe to http and filesystem.
                    HttpUtility.UrlEncode(formFile.FileName)),
                FileMode.Create);
            await formFile.CopyToAsync(fs);
        }

        return Ok();
    }
}