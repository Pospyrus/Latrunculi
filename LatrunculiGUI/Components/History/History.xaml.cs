using LatrunculiGUI.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LatrunculiGUI.Components.History
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : UserControl
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game), typeof(GameContext), typeof(History), new FrameworkPropertyMetadata(null, handleGameContextChange));

        private static void handleGameContextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var game = e.NewValue as GameContext;
            var history = d as History;
            game.Latrunculi.Desk.StepDone += (__, _) =>
            {
                if (game.Latrunculi.HistoryManager.IsLastIndexSelected)
                {
                    history.ScrollBottom();
                }
            };
        }

        public GameContext Game
        {
            get { return (GameContext)GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }

        public History()
        {
            InitializeComponent();
        }

        private void handleHistoryStartItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            Game.Latrunculi.HistoryManager.GoTo(-1);
        }

        private void handleHistoryItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is EnumeratorItem eItem)
            {
                Game.Latrunculi.HistoryManager.GoTo(eItem.Index);
            }
        }

        public void ScrollBottom()
        {
            historyScroll.ScrollToVerticalOffset(historyScroll.ExtentHeight);
        }
    }
}
