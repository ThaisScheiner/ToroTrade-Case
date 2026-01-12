using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using ToroTrade.Domain.Entities;
using ToroTrade.Domain.Interfaces;

namespace ToroTrade.Infrastructure.Persistence
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            // O ToListAsync retorna todos os registros da tabela em memória
            return await _context.Orders.ToListAsync();
        }
    }
}