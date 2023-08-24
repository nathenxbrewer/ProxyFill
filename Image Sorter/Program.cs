// See https://aka.ms/new-console-template for more information


using ProxyFill.Domain;
using ProxyFill.Domain.Services;

var path = "/Users/nathenbrewer/Documents/Pokemon Proxies/ToDo/Scarlet & Violet";
var files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
var service = new PokemonAPIService();
foreach (var file in files)
{
    if (file.Contains(".DS_Store")) continue;
    try
    {
        var filename = Path.GetFileNameWithoutExtension(file);
        var fullFileName = Path.GetFileName(file);
        var card = Helpers.ParsePTCGO(filename);
        var pokemonCard = await service.GetPokemonCard(card.Name, card.SetCode, card.CardNumber);
        if (pokemonCard == null) continue;
        var set = pokemonCard.Set.PtcgoCode ?? pokemonCard.Set.Id.ToUpper();
        var folderName = $"{pokemonCard.Set.Name} ({set.ToUpper()})";
        var folderPath = Path.Combine(path, folderName, pokemonCard.Supertype);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        var newPath = Path.Combine(path, folderPath,fullFileName);
        File.Move(file, newPath);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

}
Console.WriteLine("Hello, World!");