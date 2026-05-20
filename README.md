# Cartsys — Gerenciamento de Desenvolvedores

Desafio técnico desenvolvido por **Pedro Cruz Santos Junior** para a vaga de Desenvolvedor Full Stack na Cartsys Software.

**Contato:** pedrocruzsantosjunior@gmail.com · [LinkedIn](https://linkedin.com/in/pedrosantosfullstack)

---

## Acesso ao sistema

| Campo  | Valor               |
|--------|---------------------|
| E-mail | admin@cartsys.com   |
| Senha  | Admin@123           |

> O sistema cria automaticamente o usuário administrador e dados de exemplo na primeira execução.

---

## Tecnologias utilizadas

**Backend**
- .NET 8 / C# 12
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- JWT Authentication (BCrypt + HS256)
- QuestPDF — geração de relatório PDF
- Swagger / OpenAPI

**Frontend**
- Next.js 14 (App Router)
- React 18
- TypeScript
- Tailwind CSS
- shadcn/ui
- react-hook-form + Zod
- Axios

**Infraestrutura**
- Docker + docker-compose
- xUnit + Moq + FluentAssertions (testes unitários)

---

## Como executar

### Opção 1 — Docker (recomendado, zero configuração)

```bash
docker-compose up -d
```

Aguarde os containers iniciarem (~30s) e acesse:

| Serviço   | URL                           |
|-----------|-------------------------------|
| Frontend  | http://localhost:3000         |
| API       | http://localhost:5000         |
| Swagger   | http://localhost:5000/swagger |

### Opção 2 — Sem Docker

**Pré-requisitos:** .NET 8 SDK, Node.js 20+, SQL Server

**Backend**

```bash
cd backend

# Instalar ferramenta EF Core (primeira vez)
dotnet tool install --global dotnet-ef

# Criar banco e aplicar migrations
dotnet ef database update --project src/Infrastructure --startup-project src/Api

# Rodar a API
dotnet run --project src/Api
```

> Ajuste a connection string em `backend/src/Api/appsettings.json` se necessário.

**Frontend**

```bash
cd frontend
npm install
npm run dev
```

---

## Estrutura do projeto

```
cartsys-challenge/
├── backend/
│   ├── src/
│   │   ├── Api/            # Controllers, Middleware, Program.cs, Swagger, JWT
│   │   ├── Application/    # DTOs, Interfaces, Services, Result Pattern
│   │   ├── Domain/         # Entities, Enums, IRepository
│   │   └── Infrastructure/ # EF Core, DbContext, Repositories, Seed, BCrypt
│   └── tests/
│       └── Cartsys.Tests/  # Testes unitários (xUnit + Moq + FluentAssertions)
├── frontend/
│   └── src/
│       ├── app/            # Páginas — Next.js App Router
│       │   ├── (auth)/     # Login
│       │   └── (dashboard)/# Desenvolvedores, Linguagens, Cidades, Estados, Usuários
│       ├── components/     # Layout, shadcn/ui, componentes compartilhados
│       ├── contexts/       # AuthContext (JWT)
│       ├── lib/            # API client (Axios), utilitários
│       └── types/          # TypeScript types
├── docker-compose.yml
└── README.md
```

---

## Decisões técnicas

**Clean Architecture**
As camadas foram separadas para garantir baixo acoplamento: Domain sem dependências externas, Application dependendo apenas de Domain, Infrastructure implementando as interfaces, e Api como ponto de entrada. Facilita a substituição de banco, frameworks ou serviços externos sem tocar nas regras de negócio.

**Result Pattern**
Em vez de lançar exceções para fluxos de negócio (e-mail duplicado, registro não encontrado), os serviços retornam `Result<T>` com status code e mensagem. O controller apenas converte o resultado para `IActionResult` via extension method, mantendo os controllers simples.

**Soft Delete via Global Query Filter**
O EF Core filtra automaticamente registros com `DeletedAt != null` em todas as queries. Não é necessário adicionar `.Where(x => !x.IsDeleted)` em cada consulta — o filtro é transparente.

**Generic Repository + Repositórios Especializados**
`GenericRepository<T>` cobre as operações comuns (GetById, GetPaged, Add, Update, SoftDelete). Para entidades que precisam de queries com `Include()` (Developer, City), foram criados repositórios especializados que herdam do genérico e sobrescrevem apenas o necessário.

**IPasswordHasher como interface**
BCrypt.Net é uma dependência externa de Infrastructure. Para manter o Application desacoplado, `IPasswordHasher` é definido em Application e implementado em Infrastructure — princípio da inversão de dependência aplicado.

**JWT com 8 horas de expiração**
Token renovado a cada login. O interceptor do Axios redireciona automaticamente para `/login` em caso de 401.
