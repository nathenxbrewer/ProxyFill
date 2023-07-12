namespace ProxyFill.Pages;

public partial class Cards
{
    private List<ProxyCard> Elements = new();

    private ProxyCard selectedItem;
    private string searchString { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Elements = Data.ProxyCards;
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
        if (element.CardId.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }
    
}