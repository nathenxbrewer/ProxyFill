namespace ProxyFill.Model
{
    public class GoogleDriveImage : Image
    {
        public GoogleDriveImage(string driveId)
        {
            Thumbnail = $"https://lh3.googleusercontent.com/d/{driveId}";
            Download = $"https://drive.google.com/uc?export=download&id={driveId}";
        }
        public GoogleDriveImage(string name, string author, string driveId)
        {
            Name = name;
            Author = author;
            Thumbnail = $"https://lh3.googleusercontent.com/d/{driveId}";
            Download = $"https://drive.google.com/uc?export=download&id={driveId}";
        }
    }
}