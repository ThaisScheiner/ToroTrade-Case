using System;
using System.Threading.Tasks;
using ToroTrade.Domain.Entities;

namespace ToroTrade.Domain.Interfaces
{
    public interface IOrderRepository
    {
        // Métodos assíncronos (Task) pois acesso a banco sempre demora
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task UpdateAsync(Order order);

        Task<IEnumerable<Order>> GetAllAsync();
    }
}