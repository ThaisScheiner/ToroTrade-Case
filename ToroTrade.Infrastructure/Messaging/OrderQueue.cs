using System.Threading.Channels;
using ToroTrade.Domain.Entities;
using ToroTrade.Domain.Interfaces;

namespace ToroTrade.Infrastructure.Messaging
{
    public class OrderQueue : IOrderQueue
    {
        // O Channel funciona como uma fila Thread-Safe (segura para várias requisições simultâneas)
        private readonly Channel<Order> _channel;

        public OrderQueue()
        {
            // Cria uma fila sem limite de tamanho (Unbounded)
            _channel = Channel.CreateUnbounded<Order>();
        }

        // Método PRODUCER: Quem chama isso está "postando" uma mensagem na fila
        public async Task EnqueueAsync(Order order)
        {
            await _channel.Writer.WriteAsync(order);
        }

        // Método CONSUMER: Quem chama isso fica "ouvindo" a fila para pegar trabalho
        public IAsyncEnumerable<Order> DequeueAsync(CancellationToken ct)
        {
            return _channel.Reader.ReadAllAsync(ct);
        }
    }
}