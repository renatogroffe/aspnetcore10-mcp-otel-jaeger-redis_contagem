# aspnetcore10-mcp-otel-jaeger-redis_contagem
Implementação em ASP.NET Core + .NET 10 de MCP Server para contagem de acessos a partir de uma instância do Redis. Inclui o uso de um script do Docker Compose para monitoramento/observabilidade com OpenTelemetry + Jaeger.

---

## Testes

Testes deste MCP Server com Visual Studio Code + GitHub Copilot:

![Testes com Visual Studio Code + GitHub Copilot](img/vscode-01.png)


Um dos traces gerados no Jaeger, a partir do uso de OpenTelemetry:

![Visualização do trace gerado no Jaeger](img/jaeger-01.png)