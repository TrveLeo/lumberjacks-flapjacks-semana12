namespace FlapjacksWpf;

// Flapjack já preparado, aguardando entrega ao lumberjack que o pediu
public class FlapjackPronto
{
    public TipoFlapjack Tipo { get; }

    // Pedido ao qual este flapjack pertence
    public Pedido Pedido { get; }

    public FlapjackPronto(TipoFlapjack tipo, Pedido pedido)
    {
        Tipo = tipo;
        Pedido = pedido;
    }

    public override string ToString() => $"{Tipo} para {Pedido.Cliente.Nome}";
}
