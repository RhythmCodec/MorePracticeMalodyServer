namespace MorePracticeMalodyServer.Model.DataModel;

public class SkinInfo
{
    public int Id { get; set; }
    public int Uid { get; set; }
    public string Creator { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Cover { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public int Hot { get; set; }
    public int Time { get; set; }
    public int Mode { get; set; }
}