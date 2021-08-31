using LatrunculiCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LatrunculiGUI.Components.History
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game), typeof(GameContext), typeof(History), new FrameworkPropertyMetadata(null, handleGameContextChange));

        private static void handleGameContextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var game = e.NewValue as GameContext;
            var history = d as History;
            game.Latrunculi.Desk.StepDone += (__, _) => history.ScrollBottom();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameContext Game
        {
            get { return (GameContext)GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }

        public History()
        {
            InitializeComponent();
        }

        private void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void handleHistoryStartItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            Game.Latrunculi.HistoryManager.GoTo(-1);
        }

        private void handleHistoryItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            int index = ((sender as Grid)?.DataContext as dynamic)?.Index;
            Game.Latrunculi.HistoryManager.GoTo(index);
        }

        public void ScrollBottom()
        {
            historyScroll.ScrollToVerticalOffset(historyScroll.ExtentHeight);
        }
    }
}
