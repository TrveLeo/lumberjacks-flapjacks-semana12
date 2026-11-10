using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace FlapjacksWpf;

// Janela principal: cria lumberjacks e pedidos, aciona a cozinha e exibe o resultado
public partial class MainWindow : Window
{
    // Cozinha que atende todos os pedidos
    private readonly Cozinha cozinha = new();

    // Lumberjacks cadastrados (a lista notifica o ListBox sozinha)
    private readonly ObservableCollection<Lumberjack> lumberjacks = new();

    // Mensagens exibidas no registro de eventos
    private readonly ObservableCollection<string> registro = new();

    // Temporizador que dispara os ciclos da cozinha no modo automático
    private readonly DispatcherTimer temporizador = new() { Interval = TimeSpan.FromSeconds(2) };

    public MainWindow()
    {
        InitializeComponent();

        lumberjacksListBox.ItemsSource = lumberjacks;
        registroListBox.ItemsSource = registro;

        // Tudo que a cozinha informar vai para o ListBox de registro
        cozinha.MensagemGerada += RegistrarMensagem;
        temporizador.Tick += (remetente, argumentos) => ExecutarCiclo();
    }

    // Adiciona uma linha ao registro e rola a lista até ela
    private void RegistrarMensagem(string mensagem)
    {
        registro.Add(mensagem);
        registroListBox.ScrollIntoView(mensagem);
    }

    // Atualiza as listas que mostram o estado da cozinha e o prato de cada lumberjack
    private void AtualizarTela()
    {
        pedidosListBox.ItemsSource = cozinha.PedidosPendentes.Select(p => p.ToString()).ToList();
        prontosListBox.ItemsSource = cozinha.FlapjacksProntos.Select(f => f.ToString()).ToList();

        // Reinsere os itens para que o texto "(N no prato)" seja reavaliado
        Lumberjack? selecionado = lumberjacksListBox.SelectedItem as Lumberjack;
        List<Lumberjack> copia = lumberjacks.ToList();
        lumberjacks.Clear();
        foreach (Lumberjack l in copia)
            lumberjacks.Add(l);
        lumberjacksListBox.SelectedItem = selecionado;
    }

    // Executa um ciclo da cozinha (prepara até 3 flapjacks e entrega)
    private void ExecutarCiclo()
    {
        cozinha.ExecutarCiclo();
        AtualizarTela();
    }

    // Cadastra um novo lumberjack com o nome digitado
    private void adicionarLumberjackButton_Click(object sender, RoutedEventArgs e)
    {
        string nome = nomeTextBox.Text.Trim();
        if (nome.Length == 0)
        {
            MessageBox.Show("Digite o nome do lumberjack.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var novo = new Lumberjack(nome);
        lumberjacks.Add(novo);
        lumberjacksListBox.SelectedItem = novo;
        nomeTextBox.Clear();
        nomeTextBox.Focus();
    }

    // Permite cadastrar o lumberjack apertando Enter na caixa de texto
    private void nomeTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            adicionarLumberjackButton_Click(sender, e);
    }

    // Aceita somente dígitos nas caixas de quantidade
    private void quantidadeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    }

    // Lê a quantidade de uma caixa de texto; vazio ou inválido vale zero
    private static int LerQuantidade(TextBox caixa)
    {
        return int.TryParse(caixa.Text, out int valor) ? valor : 0;
    }

    // Monta o pedido a partir das quantidades digitadas e o envia à cozinha
    private void enviarPedidoButton_Click(object sender, RoutedEventArgs e)
    {
        if (lumberjacksListBox.SelectedItem is not Lumberjack cliente)
        {
            MessageBox.Show("Selecione um lumberjack.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Cada tipo com quantidade maior que zero vira um item do pedido
        var itens = new List<ItemPedido>();
        AdicionarItem(itens, TipoFlapjack.Crocante, LerQuantidade(crocanteTextBox));
        AdicionarItem(itens, TipoFlapjack.Mole, LerQuantidade(moleTextBox));
        AdicionarItem(itens, TipoFlapjack.Dourado, LerQuantidade(douradoTextBox));
        AdicionarItem(itens, TipoFlapjack.Banana, LerQuantidade(bananaTextBox));

        if (itens.Count == 0)
        {
            MessageBox.Show("Informe a quantidade de ao menos um tipo de flapjack.", "Aviso",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        cozinha.ReceberPedido(new Pedido(cliente, itens));

        crocanteTextBox.Text = moleTextBox.Text = douradoTextBox.Text = bananaTextBox.Text = "0";
        AtualizarTela();
    }

    // Inclui o item na lista somente se a quantidade for positiva
    private static void AdicionarItem(List<ItemPedido> itens, TipoFlapjack tipo, int quantidade)
    {
        if (quantidade > 0)
            itens.Add(new ItemPedido(tipo, quantidade));
    }

    // Aciona manualmente um ciclo da cozinha
    private void cicloButton_Click(object sender, RoutedEventArgs e)
    {
        ExecutarCiclo();
    }

    // Liga ou desliga o temporizador conforme a caixa de seleção
    private void automaticoCheckBox_Alterado(object sender, RoutedEventArgs e)
    {
        if (automaticoCheckBox.IsChecked == true)
            temporizador.Start();
        else
            temporizador.Stop();
    }

    // O lumberjack selecionado come todos os flapjacks do seu prato
    private void comerButton_Click(object sender, RoutedEventArgs e)
    {
        if (lumberjacksListBox.SelectedItem is not Lumberjack lumberjack)
        {
            MessageBox.Show("Selecione um lumberjack.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        foreach (string mensagem in lumberjack.ComerFlapjacks())
            RegistrarMensagem(mensagem);

        AtualizarTela();
    }

    // Apaga o conteúdo do registro de eventos
    private void limparButton_Click(object sender, RoutedEventArgs e)
    {
        registro.Clear();
    }
}
