namespace FlapjacksWpf;

// Cliente da cozinha. Guarda os flapjacks recebidos em uma pilha:
// o último flapjack entregue é o primeiro a ser comido.
public class Lumberjack
{
    // Pilha de flapjacks que o lumberjack ainda não comeu
    private readonly Stack<TipoFlapjack> pilhaDeFlapjacks = new();

    public string Nome { get; }

    // Quantidade de flapjacks no prato
    public int QuantidadeNoPrato => pilhaDeFlapjacks.Count;

    public Lumberjack(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome não pode ser vazio.", nameof(nome));

        Nome = nome.Trim();
    }

    // Recebe um flapjack entregue pela cozinha e o coloca no topo da pilha
    public void ReceberFlapjack(TipoFlapjack tipo)
    {
        pilhaDeFlapjacks.Push(tipo);
    }

    // Come todos os flapjacks do prato, do topo para a base.
    // Retorna uma linha de texto para cada flapjack comido.
    public List<string> ComerFlapjacks()
    {
        var mensagens = new List<string>();

        if (pilhaDeFlapjacks.Count == 0)
        {
            mensagens.Add($"{Nome} não tem flapjacks para comer.");
            return mensagens;
        }

        while (pilhaDeFlapjacks.Count > 0)
        {
            TipoFlapjack tipo = pilhaDeFlapjacks.Pop();
            mensagens.Add($"{Nome} comeu um flapjack {tipo.ToString().ToLower()}.");
        }

        return mensagens;
    }

    // Texto usado para exibir o lumberjack nos controles
    public override string ToString() => $"{Nome} ({QuantidadeNoPrato} no prato)";
}
