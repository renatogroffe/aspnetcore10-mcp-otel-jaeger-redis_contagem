namespace ContagemMcpServer.Models;

public class Contagem
{
    public int ValorAtual { get; set; }
    public string? Local { get; set; }
    public string? Kernel { get; set; }
    public string? Framework { get; set; }
    public string? Mensagem { get; set; }
}