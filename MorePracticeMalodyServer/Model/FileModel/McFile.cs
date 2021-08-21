using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Model.FileModel
{
    internal class McFile
    {
        [JsonInclude]
        public ChartMeta Meta { get; set; }

        /// <summary>
        /// Deserialize utf-8 .mc file to processable data model.
        /// </summary>
        /// <param name="path">Path to .mc file.</param>
        /// <returns></returns>
        public static McFile ParseLocalFile(string path,string fileName)
        {
            // First create our file stream.
            // This is the only way to deserialize utf8 json file.
            // // Then decompress.
            using var zipFile = ZipFile.OpenRead(path);
            var entry = zipFile.GetEntry(fileName);
            var decompressed = entry.Open();

            // Read to reader and parse.
            byte[] buffer = new byte[512000]; // Now just support 500kb.
            int len = decompressed.Read(buffer);

            return JsonSerializer.Deserialize<McFile>(buffer.AsSpan()[3..len],new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }); // Remove utf bom EF BB BF byte.
        }
    }

    class ChartMeta
    {
        [JsonInclude]
        public int Id { get; set; }
        [JsonInclude]

        public string Creator { get; set; }
        [JsonInclude]

        public string Background { get; set; }
        [JsonInclude]

        public string Version { get; set; }
        [JsonInclude]

        public int Preview { get; set; }
        [JsonInclude]

        public int Mode { get; set; }
        /// <summary>
        /// Last mod time.
        /// </summary>
        [JsonInclude]

        public int Time { get; set; }

        [JsonInclude]
        public SongMeta Song { get; set; }
    }

    class SongMeta
    {
        [JsonInclude]

        public int Id { get; set; }
        [JsonInclude]

        public string Title { get; set; }
        [JsonInclude]

        public string Artist { get; set; }
        [JsonInclude]

        public string Titleorg { get; set; }
        [JsonInclude]

        public string Artistorg { get; set; }
        /// <summary>
        /// Music file name.
        /// </summary>
        [JsonInclude]

        public string File { get; set; }
        [JsonInclude]

        public double Bpm { get; set; }
    }
}
