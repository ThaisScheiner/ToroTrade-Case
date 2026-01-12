using ToroTrade.Domain.Entities;
using ToroTrade.Domain.Interfaces;

namespace ToroTrade.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IOrderQueue _queue;

        // Injetamos o Repositório (para salvar) e a Fila (para processar depois)
        public OrderService(IOrderRepository repository, IOrderQueue queue)
        {
            _repository = repository;
            _queue = queue;
        }

        public async Task<Guid> PlaceOrderAsync(string symbol, int quantity, decimal price)
        {
            // 1. Cria a Entidade (Regra de Negócio: nasce com status Pending)
            var order = new Order(symbol, quantity, price);

            // 2. Persiste no Banco de Dados (Garantia de que o pedido existe)
            await _repository.AddAsync(order);

            // 3. Joga na Fila para processamento assíncrono
            // A API não vai esperar a ordem ser executada na B3, ela só avisa que recebeu.
            await _queue.EnqueueAsync(order);

            return order.Id;
        }
    }
}