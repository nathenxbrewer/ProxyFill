using System.Net.Http.Json;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using ProxyFill.Domain;
using ProxyFill.Domain.Services;
using ProxyFill.Model;
using ProxyFill.Shared.Dialog;
using ProxyFill.Shared.ViewModel;
using Syncfusion.Drawing;
using Image = ProxyFill.Model.Image;


namespace ProxyFill.Pages;

public partial class Index
{
    PokemonApiClient pokeClient = new PokemonApiClient("ca751a14-2892-4c5e-9460-9fe418557062");
    [Inject] public HttpClient HttpClient { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    public string CardList { get; set; }
    public bool ListSubmitted { get; set; }
    private bool OverlayVisible { get; set; }
    public string ListName { get; set; } = "Untitled List";

    public static Image DefaultCardBack = Data.CardBacks.First();

    public List<ProxyCardViewModel> Cards { get; set; } = new();
    public List<ProxyCard> ProxyCards = new();

    protected override async Task OnInitializedAsync()
    {
        ProxyCards = await Http.GetFromJsonAsync<List<ProxyCard>>("data/cards.json");
    }


    private async Task OnSubmitClick()
    {
        ListSubmitted = true;
        try
        {
            await AddCards(CardList);
        }
        catch (Exception e)
        {
            OverlayVisible = false;
            SnackBar.Add("Something went wrong trying to add cards.", Severity.Error);
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
        var convertedList = Mapper.Map<List<ProxyCardViewModel>, List<ProxyCardDTO>>(Cards);
        var cardListJson = JsonConvert.SerializeObject(convertedList, Formatting.Indented);
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
        var invalidCards = new List<string>();


        foreach (var line in cardList.Split(Environment.NewLine))
        {
            if (line.StartsWith("Pokemon")) continue;
            if (line.StartsWith("Trainer")) continue;
            if (line.StartsWith("Energy")) continue;
            if (string.IsNullOrWhiteSpace(line)) continue;
            var lineParts = line.Split(" ").ToList();
            var quantity = lineParts.First();
            lineParts.Remove(lineParts.First());
            var lineWithoutQuantity = string.Join(" ", lineParts);
            var ptcgoCard = Helpers.ParsePTCGO(lineWithoutQuantity);
            if (ptcgoCard == null)
            {
                invalidCards.Add(line);
                continue;
            }
            
            var name = ptcgoCard.Name;
            var setCode = ptcgoCard.SetCode;
            var cardNumber = ptcgoCard.CardNumber;
            var card = await PokemonAPIService.GetPokemonCard(name, setCode, cardNumber);
            if(card == null)
            {                
                invalidCards.Add(line);
                continue;
            }

            var matchingCard = card;

            //must match all 3. 
            var proxyImageRecords = ProxyCards.Where(x =>
                x.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase)
                && x.SetCode == card.Set.PtcgoCode
                && x.Number == cardNumber)
                .ToList();
            if (!proxyImageRecords.Any())
            {
                //try with setID
                proxyImageRecords = ProxyCards.Where(x =>
                    x.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase)
                    && x.SetCode.Equals(card.Set.Id, StringComparison.InvariantCultureIgnoreCase)
                    && x.Number == cardNumber).ToList();
            }
            

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
                proxyCard.SetCode = matchingCard.Set.PtcgoCode ?? matchingCard.Set.Id.ToUpper();
            }


            for (int i = 0; i < Convert.ToInt32(quantity); i++)
            {
                Cards.Add(proxyCard);
            }
        }



        OverlayVisible = false;
        if (invalidCards.Any())
        {
            var sb = new StringBuilder();
            sb.AppendLine("The following values we're invalid: <br />");
            sb.AppendJoin("<br />", invalidCards);
            var invalidCardList = sb.ToString();
            await DialogService.ShowMessageBox(
                "Warning", 
                (MarkupString) sb.ToString(), 
                yesText:"Ok");
            //Messagebox.Add("The following values we're invalid:" + string.Join(Environment.NewLine, invalidCards), Severity.Error);
        }
    }



}