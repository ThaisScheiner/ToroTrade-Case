# 🐂 ToroTrade - High Performance Trading Backend

Este projeto é uma API de simulação de Home Broker, desenvolvida para demonstrar o processamento de altas cargas de ordens de compra de ações de forma assíncrona e desacoplada.

O objetivo principal é resolver o problema de latência em horários de pico, utilizando uma arquitetura orientada a eventos com processamento em background (Non-blocking I/O).

---

## 🚀 Arquitetura e Fluxo de Dados

O projeto segue os princípios da **Clean Architecture** e **SOLID**:

1.  **API (Entrada):** O cliente envia uma ordem (`POST`) ou consulta dados (`GET`).
2.  **Fila (Channel):** Para compras, a API valida e coloca na fila em memória, retornando `202 Accepted` imediatamente.
3.  **Worker (Processamento):** Um serviço em background consome a fila, processa a regra de negócio (simulando delay da B3) e atualiza o banco.
4.  **Consulta (Query):** Os endpoints de leitura acessam o banco diretamente para entregar dados em tempo real.

### 📊 Diagrama do Fluxo

```mermaid
graph LR
    User[Usuário] -- POST Order --> API[API Controller]
    API -- Salva Pendente --> DB[(Database)]
    API -- Envia p/ Fila --> Channel{In-Memory Queue}
    API -- 202 Accepted --> User
    
    Worker[Background Worker] -- Lê Fila --> Channel
    Worker -- Processa (Delay B3) --> Worker
    Worker -- Atualiza Status --> DB
	
```

## 📚 Documentação da API (Endpoints)

Abaixo estão os exemplos de como utilizar as rotas disponíveis.

### 1️⃣ Criar Nova Ordem (Compra)
Envia uma ordem para processamento assíncrono.

* **Rota:** `POST /api/Orders`
* **Status Sucesso:** `202 Accepted`

**Body (JSON):**
```json
{
  "symbol": "PETR4",
  "quantity": 100,
  "price": 38.50
}
```

2️⃣ Listar Todas as Ordens
Retorna o histórico completo de transações.

Rota: GET /api/Orders

Status Sucesso: 200 OK

Response (JSON):

JSON

[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "symbol": "PETR4",
    "quantity": 100,
    "price": 38.50,
    "status": "Executed"
  },
  {
    "id": "a1b2c3d4-e5f6-7890-a1b2-c3d4e5f67890",
    "symbol": "VALE3",
    "quantity": 50,
    "price": 65.20,
    "status": "Executed"
  }
]
3️⃣ Buscar por Ativo (Symbol)
Filtra as ordens pelo código da ação. A busca é Case Insensitive (aceita "petr4", "PETR4" ou "Petr").

Rota: GET /api/Orders/{symbol}

Exemplo: GET /api/Orders/PETR

Status Sucesso: 200 OK

Response (JSON):

JSON

[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "symbol": "PETR4",
    "quantity": 100,
    "price": 38.50,
    "status": "Executed"
  }
]

🛠️ Tecnologias Utilizadas
Linguagem: C# (.NET 9)

Tipo de Projeto: Web API (ASP.NET Core)

Arquitetura: Clean Architecture (Onion Architecture)

Banco de Dados: In-Memory Database (EF Core) para alta velocidade em testes.

Mensageria: System.Threading.Channels (Alta performance intra-processo).

Documentação: Swagger (OpenAPI).

Logs: Serilog (Logs estruturados no console).

📦 Pacotes NuGet Instalados
Abaixo, a lista de dependências externas utilizadas em cada camada do sistema:

1. ToroTrade.API (Interface)
Swashbuckle.AspNetCore (v6.6.2): Para gerar a interface visual de documentação (Swagger).

Serilog.AspNetCore: Para observabilidade e logs detalhados do processamento do Worker.

2. ToroTrade.Infrastructure (Dados e Integrações)
Microsoft.EntityFrameworkCore.InMemory: Simula um banco de dados SQL na memória RAM, permitindo testes rápidos sem precisar instalar SQL Server local.

Microsoft.Extensions.Caching.Abstractions: Interfaces para implementar padrões de Cache (simulando Redis).

⚙️ Como Rodar o Projeto
Pré-requisitos
Visual Studio 2022 ou VS Code.

.NET SDK 9 instalado.

Passo a Passo
Clone o repositório:

```Bash

git clone https://github.com/ThaisScheiner/ToroTrade-Case.git
```

Restaure os pacotes:

```Bash

dotnet restore

```
Execute a API: Defina o projeto ToroTrade.API como inicialização e pressione F5 (ou execute dotnet run na pasta da API).

```Bash
dotnet run --project ToroTrade.API
```
Acesse o Swagger: O navegador abrirá automaticamente em: https://localhost:7091/swagger

🧪 Testando a Aplicação
Para testar o fluxo assíncrono:

Abra o Swagger.

Faça uma requisição POST em /api/Orders com o seguinte JSON:

JSON
```bash
{
  "symbol": "PETR4",
  "quantity": 100,
  "price": 38.50
}

```
Resposta Imediata: A API retornará 202 Accepted.

Verifique o Console: Você verá os logs coloridos do Worker processando a ordem em segundo plano:

🚀 Worker de Processamento iniciado... 📥 Processando ordem... ✅ Ordem EXECUTADA com sucesso!

🧠 Conceitos Aplicados (Diferenciais Técnicos)
Injeção de Dependência (DI): Uso de Containers de DI nativos do .NET (Scoped para Repositories, Singleton para a Fila).

Producer/Consumer Pattern: Implementação clássica de sistemas distribuídos usando Channels.

Hosted Services: Uso de IHostedService para tarefas que rodam durante todo o ciclo de vida da aplicação.

Assincronismo (Async/Await): Para não bloquear threads durante operações de I/O.

Desenvolvido como case técnico de estudo em Arquitetura de Software.