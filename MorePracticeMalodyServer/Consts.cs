namespace MorePracticeMalodyServer;

public static class Consts
{
    public static int API_VERSION = 202112;
    public static int MIN_SUPPORT = 202103;

    // Max items that will query.
    public static int MaxItem = 50;
}

public enum Mode
{
    Default = -1,
    Key     = 0,
    Catch   = 3,
    Pad     = 4,
    Taiko   = 5,
    Ring    = 6,
    Slide   = 7,
    Live    = 8
}

public enum Platform
{
    Windows,
    MacOS,
    Tablet,
    IPhone,
    Android,
    IPad
}