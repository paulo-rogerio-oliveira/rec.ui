using Open.recui.ViewModels;
using Open.recui.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Open.recui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rect _selectedArea ;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void AreaSelector_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SelectionWindowView();
            dialog.OnSelectedArea = (rect) =>
            {
                _selectedArea = rect;
            };
            dialog.WindowState = WindowState.Maximized;
            dialog.ShowDialog();
        }
    }
}