using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using ProxyFill.Shared.Dialog;
using ProxyFill.ViewModel;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace ProxyFill.Pages;

public partial class Index
{
    PokemonApiClient pokeClient = new PokemonApiClient("ca751a14-2892-4c5e-9460-9fe418557062");
    [Inject] public HttpClient HttpClient { get; set; }
    public string CardList { get; set; } = "1 Ball Guy SHF 65";
    public bool ListSubmitted { get; set; }
    private bool OverlayVisible { get; set; }
    public string ListName { get; set; } = "Untitled List";
    
    public static Image DefaultCardBack = Data.CardBacks.First();

    public List<ProxyCardViewModel> Cards { get; set; } = new();

    private async Task OnSubmitClick()
    {
        ListSubmitted = true;
        try
        {
            await AddCards(CardList);
        }
        catch (Exception e)
        {
        }
    }

    private async Task OnAddCardsClick()
    {
        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        var dialog = DialogService.Show<SimpleDialog>("Add Cards", options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var cardList = result.Data as string;
            await AddCards(cardList);
        }
    }

    private async Task OnRenameListClick()
    {
        var parameters = new DialogParameters()
        {
            { "TextValue", ListName }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = DialogService.Show<RenameDialog>("Rename List", parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            ListName = result.Data as string;
        }
    }

    private async void OnExportListClick()
    {
        var cardListJson = JsonConvert.SerializeObject(Cards, Formatting.Indented);
        await JSRuntime.InvokeAsync<object>("FileSaveAs", $"{ListName}.json", cardListJson);

        // using (PdfDocument pdfDocument = new PdfDocument())
        // {

        // PdfPage page = pdfDocument.Pages.Add();
        // PdfImage image = PdfImage.FromStream(stream);
        // image.Draw(page, new PointF(0, 0));
        //
        // using (MemoryStream pdfStream = new MemoryStream())
        // {
        //     //Saving the PDF document into the stream.
        //     pdfDocument.Save(pdfStream);
        //     //Closing the PDF document.
        //     pdfDocument.Close(true);
        //     await JSRuntime.InvokeAsync<object>("FileSaveAs",$"{ListName}.pdf",pdfStream);
        // }
        //}
    }

    private async Task AddCards(string cardList)
    {
        OverlayVisible = true;
        foreach (var line in cardList.Split(Environment.NewLine))
        {
            if (line.StartsWith("Pokemon")) continue;
            if (line.StartsWith("Trainer")) continue;
            if (line.StartsWith("Energy")) continue;
            if (string.IsNullOrWhiteSpace(line)) continue;
            var lineParts = line.Split(" ").ToList();
            var quantity = lineParts.First();
            var cardNumber = lineParts.Last();

            lineParts.Remove(lineParts.First());
            lineParts.Remove(lineParts.Last());

            var setCode = lineParts.Last();
            lineParts.Remove(lineParts.Last());

            var name = string.Join(" ", lineParts);

            var filter = new Dictionary<string, string>
            {
                { "name", name },
                { "set.ptcgoCode", setCode },
                { "number", cardNumber },
            };

             var cards = await pokeClient.GetApiResourceAsync<PokemonCard>(filter);
             if (cards.Count == 1) //yay, we found the match
             {
                var matchingCard = cards.Results.First();

                //must match all 3. 
                var proxyImageRecords = Data.ProxyCards.Where(x =>
                    x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    && x.SetCode == setCode
                    && x.Number == cardNumber).ToList();

                //instantiate with the given info, in case there is no match. Populates a 'card missing' image.
                var proxyCard = new ProxyCardViewModel()
                {
                    Name = name, Number = cardNumber, SetCode = setCode,
                };
                
                // var proxyCard = new ProxyCard()
                // {
                //     Name = name, Number = cardNumber, SetCode = setCode,
                //     Image = new GoogleDriveImage("14OSYdlqxV2KGKiHNLDPTBg4q2nrmmHHL")
                // };

                if (proxyImageRecords.Any())
                {
                    proxyCard.FrontImages = proxyImageRecords.Select(x => x.Image).ToList();
                    proxyCard.FrontImage = proxyCard.FrontImages.First();
                    proxyCard.Name = matchingCard.Name;
                    proxyCard.Number = matchingCard.Number;
                    proxyCard.CardId = matchingCard.Id;
                    proxyCard.BackImage = DefaultCardBack;
                }


                for (int i = 0; i < Convert.ToInt32(quantity); i++)
                {
                    Cards.Add(proxyCard);
                }

             }
        }

        OverlayVisible = false;
    }
}