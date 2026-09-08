# Biblioteca — CheckPoint 04 (FIAP · ADS · .NET)

Sistema de Gerenciamento de Biblioteca em **ASP.NET Core MVC (.NET 8)** com **Entity Framework Core** sobre **Oracle**, evoluído com **Health Checks**, **logging estruturado (Serilog + Correlation ID)**, **observabilidade (OpenTelemetry)** e **testes unitários (xUnit + Moq + FluentAssertions)**.

>  **Sobre o relacionamento Autor × Livro:** o enunciado do CP04 apresenta uma inconsistência — o resumo do CP03 e a seção "Requisitos" descrevem **1:N** ("cada autor pode ter vários livros, mas cada livro é associado a um único autor"), enquanto o parágrafo "1. Objetivo do Projeto" cita **N:N**. Este projeto implementa **1:N**, seguindo a descrição detalhada e literal dos requisitos.

## 1. Estrutura do projeto
Biblioteca/
├── Biblioteca.sln
├── src/Biblioteca.Web/ → aplicação ASP.NET Core MVC
│ ├── Models/ → Autor, Livro (entidades de domínio com validação)
│ ├── Models/ViewModels/ → modelos usados nos formulários
│ ├── Data/BibliotecaContext.cs → DbContext (EF Core → Oracle)
│ ├── Aplicacao/Repositorios/ → acesso a dados (CRUD)
│ ├── Aplicacao/Servicos/ → regras de aplicação, logging e métricas
│ ├── Aplicacao/Middlewares/ → CorrelationIdMiddleware
│ ├── Infraestrutura/Health/ → BancoDadosHealthCheck
│ ├── Infraestrutura/Observabilidade/ → AplicacaoMetricas (Meter/Counter/ActivitySource)
│ ├── Controllers/ → AutoresController, LivrosController
│ └── Views/ → CRUD completo (Index/Details/Create/Edit/Delete)
└── tests/Biblioteca.Tests.Unit/ → testes xUnit
├── Dominio/ → AutorTests, LivroTests (Fact/Theory + FluentAssertions)
└── Servicos/ → AutorServicoTests, LivroServicoTests (Moq + Verify)



## 2. Pré-requisitos

- .NET SDK 8.0
- Acesso ao Oracle da FIAP (`oracle.fiap.com.br:1521/ORCL`) com usuário/RM e senha

## 3. Configurar a connection string

O arquivo `src/Biblioteca.Web/appsettings.json` (versionado no repositório) contém apenas um placeholder:
