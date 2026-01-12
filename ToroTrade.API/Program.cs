using Serilog;
using Microsoft.EntityFrameworkCore;
using ToroTrade.API.Workers;
using ToroTrade.Application.Services;
using ToroTrade.Domain.Interfaces;
using ToroTrade.Infrastructure.Cache;
using ToroTrade.Infrastructure.Messaging;
using ToroTrade.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================
// 1. CONFIGURAÇÃO DE LOGS (Observabilidade)
// ==================================================================
// Configura o Serilog para escrever logs coloridos no Console
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ==================================================================
// 2. INJEÇÃO DE DEPENDÊNCIA 
// ==================================================================

// A. Banco de Dados (In-Memory para teste)
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("ToroDb"));

// B. Cache (Memória Distribuída simulada)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<StockPriceService>();

// C. Mensageria (A Fila)
// Singleton: A fila tem que ser ÚNICA para a aplicação inteira.
// Se fosse Scoped, cada request criaria uma fila nova e o Worker nunca receberia nada.
builder.Services.AddSingleton<IOrderQueue, OrderQueue>();

// D. Infraestrutura (Repositório)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// E. Aplicação (Service)
builder.Services.AddScoped<OrderService>();

// F. Background Worker (O Processador)
// AddHostedService registra algo que vai rodar em background assim que o app subir
builder.Services.AddHostedService<OrderProcessorWorker>();

// ==================================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Cria a documentação da API

var app = builder.Build();

// Configura o Swagger (Interface visual para testar)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();


try
{
    app.MapControllers();
    Console.WriteLine("Tentando iniciar a aplicação...");
    app.Run();
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n --- ERRO REAL DESCOBERTO --- ");
    foreach (var item in ex.LoaderExceptions)
    {
        Console.WriteLine($"- {item.Message}");
    }
    Console.WriteLine("---------------------------------\n");
    Console.ResetColor();
    // Mantém a tela aberta para você ler (caso rode sem debug)
    System.Threading.Thread.Sleep(10000);
    throw;
}
catch (Exception ex)
{
    Console.WriteLine($"Erro genérico: {ex.Message}");
    throw;
}