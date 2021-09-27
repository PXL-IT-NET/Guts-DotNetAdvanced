using System.ComponentModel;
using System.Threading;
using System.Windows;
using Exercise3.ChefAggregate;
using Exercise3.FrontDeskAggregate;

namespace Exercise3
{
    public partial class MainWindow : Window
    {
        private readonly Chef _chef;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public FrontDesk FrontDesk { get; }

        public MainWindow()
        {
            InitializeComponent();

            FrontDesk = new FrontDesk();
            _chef = new Chef(FrontDesk, new DummyChefActions(2, 1));

            _cancellationTokenSource = new CancellationTokenSource();
            _chef.StartProcessingOrders(_cancellationTokenSource.Token);

            DataContext = FrontDesk;
        }

        private void PlaceOderButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(NumberOfBurgersTextBox.Text, out int numberOfBurgers))
            {
                FrontDesk.AddOrder(numberOfBurgers);
            }
        }

        private void RemoveCompletedOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            FrontDesk.RemoveCompletedOrders();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            base.OnClosing(e);
        }
    }
}