using System.Linq;
using ProxyFill.Model;

namespace ProxyFill.Domain
{
    public static class Helpers
    {
        public static PTCGOCard ParsePTCGO(string card)
        {
            var lineParts = card.Split(" ").ToList();
            if (lineParts.Count < 3) return null;// 1 Bulbasaur DET 1
            var cardNumber = lineParts.Last();

            lineParts.Remove(lineParts.Last());

            var setCode = lineParts.Last();
            lineParts.Remove(lineParts.Last());

            var name = string.Join(" ", lineParts);
            return new PTCGOCard()
            {
                Name = name, SetCode = setCode, CardNumber = cardNumber
            };
        }
        
    }
}