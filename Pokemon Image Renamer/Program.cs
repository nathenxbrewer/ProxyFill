// See https://aka.ms/new-console-template for more information

using ProxyFill.Domain.Services;

// var path = "";
// if (args.Length == 1)
// {
//     path = args[0];
// }
// else
// {
//     Console.WriteLine("Please specify a directory:");
//     path = Console.ReadLine();
// }

var path = "/Users/nathenbrewer/Downloads/set_scarlet-violet-promos";
var files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
var service = new PokemonAPIService();
foreach (var file in files)
{
    if (file.Contains(".DS_Store")) continue;
    try
    {
        var filename = Path.GetFileNameWithoutExtension(file);
        var extension = Path.GetExtension(file);
        var setId = filename.Substring(0, 3);
        var number = filename.Substring(7, 3);
        number = number.TrimStart('0');
        var pokemonCard = await service.GetPokemonCard(setId, number);
        if (pokemonCard == null) continue;
        var set = pokemonCard.Set.PtcgoCode ?? pokemonCard.Set.Id.ToUpper();
        var newFileName = $"{pokemonCard.Name} {set} {pokemonCard.Number}{extension}";
        var newPath = Path.Combine(path, newFileName);
        if (filename != newFileName)
        {
            File.Move(file, newPath);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

}
Console.WriteLine("Hello, World!");