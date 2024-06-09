using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;

namespace Demo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CustomIndicator civm; 
        private bool isBusy;
        public event PropertyChangedEventHandler PropertyChanged;
        public MainWindow()
        {
            InitializeComponent();
            civm = new CustomIndicator();
            this.BusyIndicator.DataContext = civm; 
            IndicatorComboBox.SelectedIndex = 0;
            
            if(BusyIndicator.IsBusyAtStartup)
                Stop();
        }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async void Stop()
        {
            await Task.Delay(System.TimeSpan.FromSeconds(3));
            IsBusy = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(MyTextBox.Text, out double duration))
            {
                duration = 15;
            }

            civm.Show(new CancellationTokenSource(), true, true);
            await Task.Delay(System.TimeSpan.FromSeconds(duration));
            civm.Stop();
        }
    }
}
