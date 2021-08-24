using System;
using System.Linq;

namespace MorePracticeMalodyServer
{
    public class Util
    {
        public static void CheckVersion(int version)
        {
            if (version > Consts.API_VERSION)
                throw new NotSupportedException($"This server does not support api version {version}.");
        }

        public static bool IsCharacter(char c)
        {
            //              Japanese character               All Chinese character
            return c is (>= '\u3041' and <= '\u30FE') or (>= '\u3400' and <= '\uFA29');
        }

        public static string TrimSpecial(string input)
        {
            var charArray = input.Where(c => IsCharacter(c) || char.IsLetterOrDigit(c)).ToArray();
            return new string(charArray);
        }
    }
}