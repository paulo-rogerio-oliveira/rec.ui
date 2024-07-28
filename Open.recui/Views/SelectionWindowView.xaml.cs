using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Open.recui.Views
{
    /// <summary>
    /// Interação lógica para SelectionWindowView.xam
    /// </summary>
    public partial class SelectionWindowView : Window
    {
        private Point _startPoint;
        private Rectangle _selectionRectangle;


        public Rect SelectedArea { get; private set; }
        public Action<Rect> OnSelectedArea { get; set; }
        public Action OnCloseView { get; set; }

        public SelectionWindowView()
        {
            InitializeComponent();
            _selectionRectangle = SelectionRectangle;
            SelectionRectangle = new Rectangle();
     
        }

        private void SelectionCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(SelectionCanvas);
            _selectionRectangle.Visibility = Visibility.Visible;
            Canvas.SetLeft(_selectionRectangle, _startPoint.X);
            Canvas.SetTop(_selectionRectangle, _startPoint.Y);
            _selectionRectangle.Width = 0;
            _selectionRectangle.Height = 0;
        }

        private void SelectionCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(SelectionCanvas);
                var x = Math.Min(pos.X, _startPoint.X);
                var y = Math.Min(pos.Y, _startPoint.Y);
                var w = Math.Abs(pos.X - _startPoint.X);
                var h = Math.Abs(pos.Y - _startPoint.Y);
                _selectionRectangle.Width = w;
                _selectionRectangle.Height = h;
                Canvas.SetLeft(_selectionRectangle, x);
                Canvas.SetTop(_selectionRectangle, y);
            }
        }

        private void SelectionCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_selectionRectangle.Visibility == Visibility.Visible)
            {
                var pos = e.GetPosition(SelectionCanvas);
                var x = Math.Min(pos.X, _startPoint.X);
                var y = Math.Min(pos.Y, _startPoint.Y);
                var w = Math.Abs(pos.X - _startPoint.X);
                var h = Math.Abs(pos.Y - _startPoint.Y);
                SelectedArea = new Rect(x, y, w, h);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicializa o retângulo com preenchimento total
            var fullWidth = SelectionCanvas.ActualWidth;
            var fullHeight = SelectionCanvas.ActualHeight;
            SelectedArea = new Rect(0, 0, fullWidth, fullHeight);
            SelectionRectangle.Width = fullWidth;
            SelectionRectangle.Height = fullHeight;
            Canvas.SetLeft(SelectionRectangle, 0);
            Canvas.SetTop(SelectionRectangle, 0);
            SelectionRectangle.Visibility = Visibility.Visible;
        }
    }
}
