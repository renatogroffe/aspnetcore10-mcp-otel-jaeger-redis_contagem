using ContagemMcpServer.Models;
using ContagemMcpServer.Tracing;
using ModelContextProtocol.Server;
using StackExchange.Redis;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ContagemMcpServer.Tools;

/// <summary>
/// MCP Tool para contagem de acessos utilizando Redis.
/// </summary>
internal class ContadorTool(ConnectionMultiplexer connectionRedis)
{
    private static readonly string _LOCAL;
    private static readonly string _KERNEL;
    private static readonly string _FRAMEWORK;

    static ContadorTool()
    {
        _LOCAL = Environment.MachineName;
        _KERNEL = Environment.OSVersion.VersionString;
        _FRAMEWORK = RuntimeInformation.FrameworkDescription;
    }

    private readonly ConnectionMultiplexer _connectionRedis = connectionRedis;

    [McpServerTool]
    [Description("Retorna o resultado de um contador sequencial mantido " +
        "atraves de uma instancia do Redis.")]
    public async Task<Result<Contagem>> GerarNovoValorContagem()
    {
        try
        {
            using var activity1 = OpenTelemetryExtensions.ActivitySource
                .StartActivity("GerarValorContagemRedis")!;
            int valorAtualContador = (int)_connectionRedis.GetDatabase()
                .StringIncrement("ContagemMcpServer");
            activity1.SetTag("valorAtualRedis", valorAtualContador);
            activity1.SetTag("horario", $"{DateTime.UtcNow.AddHours(-3):HH:mm:ss}");
            activity1.Stop();

            var result = new Result<Contagem>
            {
                Data = new Contagem()
                {
                    ValorAtual = valorAtualContador,
                    Local = _LOCAL,
                    Kernel = _KERNEL,
                    Mensagem = "ContagemMcpServer - testes com Redis",
                    Framework = _FRAMEWORK
                },
                Message = $"Um novo incremento na contagem foi realizado com sucesso!"
            };
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return new Result<Contagem>
            {
                IsSuccess = false,
                Message = $"Erro durante a geracao de um incremento na contagem: {ex.Message}"
            };
        }
    }
}