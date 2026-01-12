using Microsoft.EntityFrameworkCore;
using ToroTrade.Domain.Entities;

namespace ToroTrade.Infrastructure.Persistence
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        // Representa a tabela "Orders" no banco
        public DbSet<Order> Orders { get; set; }
    }
}