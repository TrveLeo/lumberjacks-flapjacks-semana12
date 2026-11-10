namespace FlapjacksWpf;

// Um item de um pedido: um tipo de flapjack e a quantidade que falta preparar
public class ItemPedido
{
    // Tipo de flapjack solicitado
    public TipoFlapjack Tipo { get; }

    // Quantidade ainda não preparada pela cozinha
    public int QuantidadeRestante { get; private set; }

    public ItemPedido(TipoFlapjack tipo, int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve ser maior que zero.");

        Tipo = tipo;
        QuantidadeRestante = quantidade;
    }

    // Marca uma unidade deste item como preparada
    public void DarBaixa()
    {
        if (QuantidadeRestante > 0)
            QuantidadeRestante--;
    }
}
