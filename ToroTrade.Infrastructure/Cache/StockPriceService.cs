using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace ToroTrade.Infrastructure.Cache
{
    public class StockPriceService
    {
        private readonly IDistributedCache _cache;

        public StockPriceService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            // Tenta buscar o preço no Cache (Redis) pela chave "price:PETR4"
            string? cachedPrice = await _cache.GetStringAsync($"price:{symbol}");

            if (!string.IsNullOrEmpty(cachedPrice))
            {
                // Se achou no cache, retorna rápido (Simulação de Alta Performance)
                return decimal.Parse(cachedPrice);
            }

            // Se não achou (Cache Miss), simula que foi na B3 pegar o preço
            // Na vida real, aqui chamaria uma API externa lenta
            decimal fakePrice = new Random().Next(20, 50) + (decimal)new Random().NextDouble();

            // Salva no cache para a próxima consulta ser rápida (Expira em 10 segundos)
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

            await _cache.SetStringAsync($"price:{symbol}", fakePrice.ToString(), options);

            return Math.Round(fakePrice, 2);
        }
    }
}