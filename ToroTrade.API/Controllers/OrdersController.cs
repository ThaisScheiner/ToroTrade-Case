using Microsoft.AspNetCore.Mvc;
using ToroTrade.Application.Services;

namespace ToroTrade.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        // Injetamos o Application Service (a camada de inteligência)
        public OrdersController(OrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            // Validação básica 
            if (request.Quantity <= 0) return BadRequest("Quantidade deve ser maior que zero.");

            // Chama o serviço para criar a ordem
            var orderId = await _service.PlaceOrderAsync(request.Symbol, request.Quantity, request.Price);

            
            // Não retornamos "200 OK" nem "201 Created".
            // Retornamos "202 Accepted" porque a ordem foi aceita na FILA,
            // mas ainda não foi executada na Bolsa. Isso é arquitetura assíncrona real.
            return Accepted(new
            {
                OrderId = orderId,
                Status = "Recebido - Processando em Background",
                Timestamp = DateTime.UtcNow
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _service.GetAllOrdersAsync();
            return Ok(orders);
        }
    }

    // DTO (Data Transfer Object) - O modelo do JSON que chega
    // O 'record' é perfeito para DTOs (leve e imutável)
    public record CreateOrderRequest(string Symbol, int Quantity, decimal Price);
}