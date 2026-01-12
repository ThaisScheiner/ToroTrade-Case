using ToroTrade.Domain.Entities;
using ToroTrade.Domain.Interfaces;
using ToroTrade.Infrastructure.Messaging;

namespace ToroTrade.API.Workers
{
    // BackgroundService é uma classe base do .NET para tarefas que rodam em loop infinito
    public class OrderProcessorWorker : BackgroundService
    {
        private readonly IOrderQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderProcessorWorker> _logger;

        public OrderProcessorWorker(IOrderQueue queue, IServiceProvider serviceProvider, ILogger<OrderProcessorWorker> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker de Processamento iniciado...");

            // Fica "ouvindo" a fila para sempre (enquanto a aplicação rodar)
            await foreach (var order in _queue.DequeueAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation($"Processando ordem {order.Id} para {order.Symbol}...");

                    // Simula o tempo de processamento da Bolsa (B3)
                    await Task.Delay(2000, stoppingToken);

                    // IMPORTANTE: Como o Worker é Singleton e o Repository é Scoped (Banco),
                    // precisamos criar um escopo manual para pegar o repositório.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                        // Busca a ordem atualizada no banco (opcional, mas seguro)
                        var orderDb = await repository.GetByIdAsync(order.Id);

                        if (orderDb != null)
                        {
                            // Regra de Negócio: Executar a ordem
                            orderDb.Execute();

                            // Atualiza no banco
                            await repository.UpdateAsync(orderDb);

                            _logger.LogInformation($"✅ Ordem {order.Id} EXECUTADA com sucesso!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao processar ordem {order.Id}");
                }
            }
        }
    }
}