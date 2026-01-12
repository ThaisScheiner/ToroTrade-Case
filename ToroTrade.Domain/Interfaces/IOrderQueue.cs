using ToroTrade.Domain.Entities;

namespace ToroTrade.Domain.Interfaces
{
    public interface IOrderQueue
    {
        Task EnqueueAsync(Order order);

        //Para o Worker conseguir ler a fila através da interface
        IAsyncEnumerable<Order> DequeueAsync(CancellationToken ct);
    }
}