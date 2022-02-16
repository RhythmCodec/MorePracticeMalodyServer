using System;
using System.Linq;

namespace MorePracticeMalodyServer;

public class Util
{
    public static void CheckVersion(int version)
    {
        if (version > Consts.API_VERSION || version < Consts.MIN_SUPPORT)
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

    public static long GetTimeStamp(DateTime time) // Get specified timestamp.
    {
        var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        var nowTime = time;
        var unixTime = (long)Math.Round((nowTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
        return unixTime;
    }
}