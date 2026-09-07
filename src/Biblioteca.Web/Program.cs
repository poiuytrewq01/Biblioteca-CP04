using Biblioteca.Web.Aplicacao.Middlewares;
using Biblioteca.Web.Aplicacao.Repositorios;
using Biblioteca.Web.Aplicacao.Servicos;
using Biblioteca.Web.Data;
using Biblioteca.Web.Infraestrutura.Health;
using Biblioteca.Web.Infraestrutura.Observabilidade;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

// Logger "bootstrap": captura qualquer erro que ocorra antes do host terminar de ser configurado.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando a aplicação Biblioteca.API...");

    var builder = WebApplication.CreateBuilder(args);

    // ----- Serilog: provedor global de logs (Console + Arquivo rotativo) -----
    builder.Host.UseSerilog((contexto, servicos, configuracaoLog) => configuracaoLog
        .ReadFrom.Configuration(contexto.Configuration)
        .ReadFrom.Services(servicos)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/app-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}"));

    // ----- MVC -----
    builder.Services.AddControllersWithViews();

    // ----- Banco de dados Oracle via Entity Framework Core -----
    var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

    builder.Services.AddDbContext<BibliotecaContext>(options =>
        options.UseOracle(connectionString));

    // ----- Injeção de dependência: repositórios e serviços de aplicação -----
    builder.Services.AddScoped<IAutorRepositorio, AutorRepositorio>();
    builder.Services.AddScoped<ILivroRepositorio, LivroRepositorio>();
    builder.Services.AddScoped<IAutorServico, AutorServico>();
    builder.Services.AddScoped<ILivroServico, LivroServico>();

    // ----- Health Checks: expõe /health validando a conexão com o Oracle -----
    builder.Services.AddHealthChecks()
        .AddCheck<BancoDadosHealthCheck>("banco_de_dados_oracle");

    // ----- OpenTelemetry: Tracing + Métricas -----
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(recurso => recurso.AddService(AplicacaoMetricas.NomeServico))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(AplicacaoMetricas.NomeServico)
            .AddConsoleExporter())
        .WithMetrics(metricas => metricas
            .AddAspNetCoreInstrumentation()
            .AddMeter(AplicacaoMetricas.NomeServico)
            .AddConsoleExporter());

    var app = builder.Build();

    // Configure o pipeline de requisições HTTP.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // Log estruturado de cada requisição HTTP (método, rota, status, tempo de resposta).
    app.UseSerilogRequestLogging();

    // Lê/gera o X-Correlation-ID e injeta no contexto de log — deve vir cedo no pipeline.
    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    // Endpoint nativo de diagnósticos de saúde.
    app.MapHealthChecks("/health");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação Biblioteca.API falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}
