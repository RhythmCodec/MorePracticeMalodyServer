using System.Collections.Generic;

namespace MorePracticeMalodyServer.Model
{
    public class SignResponse
    {
        public int Code { get; set; } = 0;
        public int ErrorIndex { get; set; } = -1;
        public string ErrorMsg { get; set; }
        public string Host { get; set; }
        public List<object> Meta { get; set; } = new();
    }
}