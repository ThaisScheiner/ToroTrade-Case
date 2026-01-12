using System;

namespace ToroTrade.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string Symbol { get; private set; } // Ex: PETR4, VALE3
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public string Status { get; private set; } // Pending, Executed, Failed
        public DateTime CreatedAt { get; private set; }

        // Construtor: Obriga a passar os dados para criar uma Ordem
        public Order(string symbol, int quantity, decimal price)
        {
            Id = Guid.NewGuid();
            Symbol = symbol;
            Quantity = quantity;
            Price = price;
            Status = "Pending"; // Nasce sempre como Pendente
            CreatedAt = DateTime.UtcNow;
        }

        // Métodos para alterar o status (Regra de Negócio)
        public void Execute() => Status = "Executed";
        public void Fail() => Status = "Failed";
    }
}