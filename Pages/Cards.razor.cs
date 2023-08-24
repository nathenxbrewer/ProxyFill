using System.Data;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using MudBlazor;
using Newtonsoft.Json;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using ProxyFill.Domain;
using ProxyFill.Model;
using ProxyFill.Shared.Dialog;

namespace ProxyFill.Pages;

public partial class Cards
{
    private List<ProxyCard> ProxyCards = new();

    private ProxyCard selectedItem;
    private string searchString { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ProxyCards = await Http.GetFromJsonAsync<List<ProxyCard>>("data/cards.json");
    }

    private bool FilterFunc1(ProxyCard element) => FilterFunc(element, searchString);

    private bool FilterFunc(ProxyCard element, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.SetCode.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Number.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Image.Download.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }
    
    private async Task OnAddCardClicked()
    {
        var dialog = DialogService.Show<AddCardDialog>();
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var card = result.Data as ProxyCard;
            ProxyCards.Add(card);
            Snackbar.Add("Card added successfully", Severity.Success);
            StateHasChanged();
        }

    }
    


    private async Task OnAddDriveClicked()
    {
        var cardsAdded = 0;
       
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true};
        var dialog = DialogService.Show<AddDriveFolderDialog>("Add Drive Folder", options);
        var result = await dialog.Result;
        if (result.Cancelled) return;

        var data = result.Data as string[];
        var author = data[0];
        var folderId = data[1];
        using (var httpclient = new HttpClient())
        {
            var nextPageToken = "";
            var apiKey = "AIzaSyBOGAtxTDZMJas_EkIRb0pVBpyQYpTaHXU";
            var query = '"' + folderId + '"' + " in parents";
            var url = $"https://www.googleapis.com/drive/v3/files?q={query}&key={apiKey}";
            var list = await httpclient.GetFromJsonAsync<DriveSearchFileResponse>(url);
            nextPageToken = list.nextPageToken;
            while(!string.IsNullOrEmpty(nextPageToken))
            {
                url = $"https://www.googleapis.com/drive/v3/files?q={query}&key={apiKey}&pageToken={list.nextPageToken}";
                var newList = await httpclient.GetFromJsonAsync<DriveSearchFileResponse>(url);
                list.files = list.files.Concat(newList.files).ToArray();
                nextPageToken = newList.nextPageToken;
            }
            
            var filesList = new List<ProxyFill.Model.File>();
            foreach (var record in list.files)
            {
                if (record.mimeType == "image/png")
                {
                    filesList.Add(record);
                }
                else if (record.mimeType == "application/vnd.google-apps.folder")
                {
                    folderId = record.id;
                    query = '"' + folderId + '"' + " in parents";
                    url = $"https://www.googleapis.com/drive/v3/files?q={query}&key={apiKey}";
                    var newlist = await httpclient.GetFromJsonAsync<DriveSearchFileResponse>(url);
                    nextPageToken = newlist.nextPageToken;
                    while(!string.IsNullOrEmpty(nextPageToken))
                    {
                        url = $"https://www.googleapis.com/drive/v3/files?q={query}&key={apiKey}&pageToken={newlist.nextPageToken}";
                        var newList = await httpclient.GetFromJsonAsync<DriveSearchFileResponse>(url);
                        newlist.files = newlist.files.Concat(newList.files).ToArray();
                        nextPageToken = newList.nextPageToken;
                    }
                    foreach (var newRecord in newlist.files)
                    {
                        if (newRecord.mimeType == "image/png")
                        {
                            filesList.Add(newRecord);
                        }
                    }
                }
            }
            
            foreach (var file in filesList)
            {
                var ptcgoCard = Helpers.ParsePTCGO(Path.GetFileNameWithoutExtension(file.name));

                if (ptcgoCard == null) 
                    continue;
                
                //check if we already have this file. 
                var downloadURL = new GoogleDriveImage(file.id);
                if(ProxyCards.Select(x=>x.Image).Any(x=>x.Download == downloadURL.Download)) continue;
                
                var matchingCard = await PokemonAPIService.GetPokemonCard(ptcgoCard.Name, ptcgoCard.SetCode, ptcgoCard.CardNumber);
                if (matchingCard == null)
                    continue;
                
                ProxyCards.Add(new()
                {
                    Name = matchingCard.Name, SetCode = matchingCard.Set.PtcgoCode ?? matchingCard.Set.Id.ToUpper(), Number = matchingCard.Number, 
                    Image = new GoogleDriveImage(file.name,author, file.id)
                });
                Console.WriteLine($"Added Card: {file.name}");
                cardsAdded++;
            }
            Snackbar.Add($"Done adding Drive folder, {cardsAdded}/{filesList.Count} added.", Severity.Success);
        }
    }
    
}