# Biblioteca — CheckPoint 04 (FIAP · ADS · .NET)

Sistema de Gerenciamento de Biblioteca em **ASP.NET Core MVC (.NET 8)** com **Entity Framework Core** sobre **Oracle**, evoluído com **Health Checks**, **logging estruturado (Serilog + Correlation ID)**, **observabilidade (OpenTelemetry)** e **testes unitários (xUnit + Moq + FluentAssertions)**.

>  **Sobre o relacionamento Autor × Livro:** o enunciado do CP04 tem uma inconsistência — o resumo do CP03 e a seção "Requisitos" descrevem **1:N** ("cada autor pode ter vários livros, mas cada livro é associado a um único autor"), mas o parágrafo "1. Objetivo do Projeto" cita **N:N**. Este projeto implementa **1:N**, seguindo a descrição detalhada e literal dos requisitos. Se o professor confirmar que quer N:N, me avise que eu ajusto (é uma mudança pontual: trocar a FK única por uma tabela de junção `AUTOR_LIVRO`).

## 1. Estrutura do projeto

```
Biblioteca/
├── Biblioteca.sln
├── src/Biblioteca.Web/              → aplicação ASP.NET Core MVC
│   ├── Models/                      → Autor, Livro (entidades de domínio com validação)
│   ├── Models/ViewModels/           → modelos usados nos formulários
│   ├── Data/BibliotecaContext.cs    → DbContext (EF Core → Oracle)
│   ├── Aplicacao/Repositorios/      → acesso a dados (CRUD)
│   ├── Aplicacao/Servicos/          → regras de aplicação, logging e métricas
│   ├── Aplicacao/Middlewares/       → CorrelationIdMiddleware
│   ├── Infraestrutura/Health/       → BancoDadosHealthCheck
│   ├── Infraestrutura/Observabilidade/ → AplicacaoMetricas (Meter/Counter/ActivitySource)
│   ├── Controllers/                 → AutoresController, LivrosController
│   └── Views/                       → CRUD completo (Index/Details/Create/Edit/Delete)
└── tests/Biblioteca.Tests.Unit/     → testes xUnit
    ├── Dominio/                     → AutorTests, LivroTests (Fact/Theory + FluentAssertions)
    └── Servicos/                    → AutorServicoTests, LivroServicoTests (Moq + Verify)
```

## 2. Pré-requisitos

- .NET SDK 8.0
- Acesso ao Oracle da FIAP (`oracle.fiap.com.br:1521/ORCL`) com seu usuário/RM e senha

## 3. Configurar a connection string (IMPORTANTE)

O arquivo `src/Biblioteca.Web/appsettings.json` (esse **vai** para o GitHub) tem só um placeholder:

```
User Id=SEU_RM;Password=SUA_SENHA;Data Source=...
```

Suas credenciais reais já estão preenchidas em `src/Biblioteca.Web/appsettings.Development.json`, que **está no `.gitignore`** — ou seja, não vai para o repositório público. Isso é proposital: sua senha do Oracle da FIAP não deve aparecer em um repositório público no GitHub. **Não remova essa entrada do `.gitignore` e não copie a senha para o `appsettings.json`.**

Ao rodar localmente com `dotnet run` (ambiente `Development` por padrão), o `appsettings.Development.json` é lido automaticamente por cima do `appsettings.json`.

## 4. Rodar o projeto

```bash
cd Biblioteca

# Restaurar pacotes (baixa Oracle EF Core, Serilog, OpenTelemetry, xUnit, Moq, FluentAssertions)
dotnet restore

# Instalar a ferramenta de migrations do EF Core (uma vez só, globalmente)
dotnet tool install --global dotnet-ef

# Criar a migration inicial (gera as tabelas AUTORES e LIVROS)
dotnet ef migrations add MigracaoInicial --project src/Biblioteca.Web

# Aplicar no banco Oracle
dotnet ef database update --project src/Biblioteca.Web

# Rodar a aplicação
dotnet run --project src/Biblioteca.Web
```

Acesse:
- `https://localhost:{porta}/` — tela inicial
- `https://localhost:{porta}/Autores` — CRUD de autores
- `https://localhost:{porta}/Livros` — CRUD de livros
- `https://localhost:{porta}/health` — endpoint de diagnóstico (deve retornar `Healthy` se a conexão com o Oracle estiver OK)

Os logs aparecem no console e também são gravados em `src/Biblioteca.Web/logs/app-*.log` (pasta criada automaticamente, ignorada pelo git).

## 5. Rodar os testes

```bash
dotnet test
```

Cobre: criação/validação das entidades `Autor` e `Livro` (`[Fact]`/`[Theory]` + `FluentAssertions`), e os serviços `AutorServico`/`LivroServico` com repositórios mockados via `Moq`, verificando chamadas com `Verify(..., Times.Once)`, seguindo o padrão **Arrange-Act-Assert**.

## 6. Sobre cada requisito do CP04

| Requisito | Onde está |
|---|---|
| Health Check customizado | `Infraestrutura/Health/BancoDadosHealthCheck.cs`, mapeado em `/health` no `Program.cs` |
| Correlation ID + Serilog | `Aplicacao/Middlewares/CorrelationIdMiddleware.cs` + configuração do Serilog no `Program.cs` (Console + arquivo rotativo em `logs/app-.log`) |
| OpenTelemetry (Tracing + Métricas) | `Infraestrutura/Observabilidade/AplicacaoMetricas.cs` (Meter/Counter/ActivitySource) + `AddOpenTelemetry()` no `Program.cs`, com spans manuais e incremento do contador nos serviços de criação/atualização/remoção |
| Testes unitários (xUnit/Moq/FluentAssertions, AAA) | `tests/Biblioteca.Tests.Unit/` |

## 7. Não consegui compilar/testar aqui

Este projeto foi escrito neste ambiente sem acesso ao NuGet (não consegui baixar os pacotes do Oracle/Serilog/OpenTelemetry/Moq/FluentAssertions para compilar e rodar `dotnet test` aqui). O código segue os padrões corretos dessas bibliotecas, mas **rode `dotnet restore && dotnet build && dotnet test` na sua máquina antes de gravar o vídeo**, para garantir que compila e os testes passam. Se aparecer algum erro, me mostre a mensagem que eu corrijo.

## 8. Entrega (o que falta você fazer)

1. Rodar tudo localmente (passo 4) e confirmar que builda, os testes passam (passo 5) e o `/health` responde `Healthy`.
2. Criar um repositório no GitHub, subir o código (o `.gitignore` já protege sua senha).
3. Escrever no README do GitHub (pode reaproveitar este arquivo) o link do vídeo.
4. Gravar o vídeo de demonstração seguindo o roteiro em `ROTEIRO_VIDEO.md` e subir no YouTube (pode ser "não listado").
5. No Teams: anexar `identificacao.txt` (fora do .zip do código) + link do vídeo + link do GitHub. **Só uma pessoa do grupo envia.**

**Confirme antes com o professor:** o enunciado diz equipes de **até 3 pessoas**, e você me passou 4 nomes (você + Kevin + Matheus + Pedro). Ou o grupo reduz para 3, ou você confirma com o professor se pode ser 4 dessa vez — ajusto o `identificacao.txt` para o que for combinado.
