using ProxyFill.Model;

namespace ProxyFill;

public static class Data
{
    // public static List<ProxyCard> ProxyCards = new()
    // {
    //     new()
    //     {
    //         Name = "Ball Guy", SetCode = "SHF", Number = "65", CardId = "swsh45-65",
    //         Image = new GoogleDriveImage("Ball Guy", "DaddyOogway","1c1HxdVgFhqwBD4MHJT9pV8QcPsmgXebN")
    //     },
    //     new()
    //     {
    //         Name = "Gardenia's Vigor", SetCode = "ASR", Number = "184", CardId = "swsh10-184",
    //         Image = new GoogleDriveImage("Gardenia's Vigor","DaddyOogway","1GmVfmxhtfcQI0kEtQvE1IfnM4Vvy66xe")
    //     },
    //     new()
    //     {
    //         Name = "Acerola", SetCode = "BUS", Number = "142", CardId = "sm3-142",
    //         Image = new GoogleDriveImage("Acerola", "DaddyOogway","1B5mK2bxbn0boQewXFNxT5TgYnGLE0dhg")
    //     },
    //     new()
    //     {
    //         Name = "Avery", SetCode = "CRE", Number = "187", CardId = "swsh6-187",
    //         Image = new GoogleDriveImage("Avery","DaddyOogway","12CD8FXVIP385Io8-yN9rRrykKm9ow-Nr")
    //     },
    // };

    public static List<Image> CardBacks = new()
    {
        new GoogleDriveImage("Pokemon MPC Back","WarpDandy", "1Ou7W04WNHEByBHQaWlYUyLWM613cYNbg")
    };
}

public class ProxyCard
{
    public string Name { get; set; }
    public string Number { get; set; }
    public string SetCode { get; set; }
    public string CardId { get; set; }
    public Image Image { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.Now;
}



