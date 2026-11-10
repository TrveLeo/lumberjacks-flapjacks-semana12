namespace FlapjacksWpf;

// Cozinha: recebe pedidos, prepara flapjacks em lotes e os entrega aos lumberjacks
public class Cozinha
{
    // Máximo de flapjacks preparados a cada ciclo
    public const int LimitePorCiclo = 3;

    // Fila de pedidos aguardando preparo (FIFO)
    private readonly Queue<Pedido> filaDePedidos = new();

    // Fila de flapjacks prontos aguardando entrega (FIFO)
    private readonly Queue<FlapjackPronto> filaDeProntos = new();

    // Disparado sempre que a cozinha tem algo a informar (substitui o Console do exercício original)
    public event Action<string>? MensagemGerada;

    // Pedidos pendentes, do mais antigo para o mais novo (somente leitura)
    public IEnumerable<Pedido> PedidosPendentes => filaDePedidos;

    // Flapjacks prontos para entrega (somente leitura)
    public IEnumerable<FlapjackPronto> FlapjacksProntos => filaDeProntos;

    // Recebe um novo pedido no fim da fila
    public void ReceberPedido(Pedido pedido)
    {
        filaDePedidos.Enqueue(pedido);
        MensagemGerada?.Invoke($"Pedido recebido de {pedido.Cliente.Nome}: {pedido} ({pedido.TotalRestante} flapjacks).");
    }

    // Prepara até 3 flapjacks, atendendo os pedidos na ordem de chegada.
    // Um pedido não precisa ficar completo para que seus flapjacks sejam entregues.
    public void Preparar()
    {
        int preparados = 0;

        while (preparados < LimitePorCiclo && filaDePedidos.Count > 0)
        {
            // Olha o pedido mais antigo sem removê-lo da fila
            Pedido pedido = filaDePedidos.Peek();
            TipoFlapjack? tipo = pedido.RetirarProximo();

            if (tipo != null)
            {
                filaDeProntos.Enqueue(new FlapjackPronto(tipo.Value, pedido));
                preparados++;
                MensagemGerada?.Invoke($"Cozinha preparou um flapjack {tipo.Value.ToString().ToLower()} para {pedido.Cliente.Nome}.");
            }

            // Pedido totalmente preparado sai da fila de pendentes
            if (pedido.Completo)
            {
                filaDePedidos.Dequeue();
                MensagemGerada?.Invoke($"Pedido de {pedido.Cliente.Nome} totalmente preparado.");
            }
        }

        if (preparados == 0)
            MensagemGerada?.Invoke("Cozinha ociosa: nenhum pedido pendente.");
    }

    // Entrega todos os flapjacks prontos aos seus lumberjacks, na ordem de preparo
    public void Entregar()
    {
        while (filaDeProntos.Count > 0)
        {
            FlapjackPronto pronto = filaDeProntos.Dequeue();
            pronto.Pedido.Cliente.ReceberFlapjack(pronto.Tipo);
            MensagemGerada?.Invoke($"Entregue flapjack {pronto.Tipo.ToString().ToLower()} a {pronto.Pedido.Cliente.Nome}.");
        }
    }

    // Um ciclo completo: prepara um lote e entrega em seguida
    public void ExecutarCiclo()
    {
        Preparar();
        Entregar();
    }
}
