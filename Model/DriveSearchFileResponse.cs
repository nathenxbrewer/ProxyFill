namespace ProxyFill.Model;

public class DriveSearchFileResponse
{
    public string kind { get; set; }
    public bool incompleteSearch { get; set; }
    public string nextPageToken { get; set; }
    public File[] files { get; set; }
}

public class File
{
    public string kind { get; set; }
    public string mimeType { get; set; }
    public string id { get; set; }
    public string name { get; set; }
}

