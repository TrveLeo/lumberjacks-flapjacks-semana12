namespace FlapjacksWpf;

// Pedido feito por um lumberjack: um ou mais tipos de flapjack, cada um com sua quantidade
public class Pedido
{
    // Itens do pedido (um por tipo de flapjack)
    private readonly List<ItemPedido> itens;

    // Lumberjack que fez o pedido e receberá os flapjacks
    public Lumberjack Cliente { get; }

    // Total de flapjacks que ainda não foram preparados
    public int TotalRestante => itens.Sum(item => item.QuantidadeRestante);

    // Indica se a cozinha já preparou tudo o que foi pedido
    public bool Completo => TotalRestante == 0;

    public Pedido(Lumberjack cliente, IEnumerable<ItemPedido> itensSolicitados)
    {
        Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
        itens = itensSolicitados?.ToList() ?? throw new ArgumentNullException(nameof(itensSolicitados));

        if (itens.Count == 0)
            throw new ArgumentException("O pedido precisa ter ao menos um item.", nameof(itensSolicitados));
    }

    // Retira do pedido o próximo flapjack a ser preparado, respeitando a ordem dos itens.
    // Retorna null caso não reste nada a preparar.
    public TipoFlapjack? RetirarProximo()
    {
        ItemPedido? item = itens.FirstOrDefault(i => i.QuantidadeRestante > 0);
        if (item == null)
            return null;

        item.DarBaixa();
        return item.Tipo;
    }

    // Descrição do que ainda falta preparar, ex.: "Ana: 2 Crocante, 1 Banana"
    public override string ToString()
    {
        var restantes = itens
            .Where(i => i.QuantidadeRestante > 0)
            .Select(i => $"{i.QuantidadeRestante} {i.Tipo}");
        return $"{Cliente.Nome}: {string.Join(", ", restantes)}";
    }
}
