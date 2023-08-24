using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace ProxyFill.Domain.Services
{
    public class PokemonAPIService
    {
        public PokemonApiClient pokeClient = new PokemonApiClient("ca751a14-2892-4c5e-9460-9fe418557062");

        public async Task<PokemonCard> GetPokemonCard(string setCode, string cardNumber)
        {
            var cards = new ApiResourceList<PokemonCard>();

            var filterPtcgo = new Dictionary<string, string>
            {
                { "set.ptcgoCode", setCode },
                { "number", cardNumber },
            };
            var filterSetID = new Dictionary<string, string>
            {
                { "set.id", setCode },
                { "number", cardNumber },
            };
            
            cards = await pokeClient.GetApiResourceAsync<PokemonCard>(filterPtcgo);
            if (cards.Results.Count == 1)
            {
                return cards.Results.First();
            }
            
            cards = await pokeClient.GetApiResourceAsync<PokemonCard>(filterSetID);
            if (cards.Results.Count == 1)
            {
                return cards.Results.First();
            }

            return null;
        }

        public async Task<PokemonCard> GetPokemonCard(string name, string setCode, string cardNumber)
        {
            var cards = new ApiResourceList<PokemonCard>();
            var filterPtcgo = new Dictionary<string, string>
            {
                { "name", name },
                { "set.ptcgoCode", setCode },
                { "number", cardNumber },
            };
        
            var filterNoSet = new Dictionary<string, string>
            {
                { "name", name },
                { "number", cardNumber },
            };
        
            var filterSetId = new Dictionary<string, string>
            {
                { "name", name },
                { "set.id", setCode },
                { "number", cardNumber },
            };

            var filterNoNameSetId = new Dictionary<string, string>
            {
                { "set.id", setCode },
                { "number", cardNumber }
            };
        
            cards = await pokeClient.GetApiResourceAsync<PokemonCard>(filterNoSet);
            if (cards.Results.Count == 1)
            {
                return cards.Results.First();
            }
            cards = await pokeClient.GetApiResourceAsync<PokemonCard>(filterPtcgo);
            if (cards.Results.Count == 1)
            {
                return cards.Results.First();
            }
            cards = await pokeClient.GetApiResourceAsync<PokemonCard>(filterSetId);
            if (cards.Results.Count == 1)
            {
                return cards.Results.First();
            }
            cards = await pokeClient.GetApiResourceAsync<PokemonCard>(filterNoNameSetId);
            if (cards.Results.Count == 1)
            {
                return cards.Results.First();
            }

            return null;
        }
    }
}