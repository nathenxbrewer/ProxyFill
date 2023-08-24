
using System.Collections.Generic;
using ProxyFill.Model;

namespace ProxyFill.Shared.ViewModel
{
    public class ProxyCardViewModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string SetCode { get; set; }
        public string CardId { get; set; }
        public Image FrontImage { get; set; }
        public Image BackImage { get; set; }

        public List<Image> FrontImages { get; set; } = new List<Image>();
    }
}