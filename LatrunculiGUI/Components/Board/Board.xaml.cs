using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Players;
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
namespace LatrunculiGUI.Components.Board
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty AppProperty = DependencyProperty.Register(nameof(App), typeof(LatrunculiApp), typeof(Board), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ActivePositionProperty = DependencyProperty.Register(nameof(ActivePosition), typeof(ChessBoxPosition), typeof(Board), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty HelpMoveProperty = DependencyProperty.Register(nameof(HelpMove), typeof(Move), typeof(Board), new FrameworkPropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        public LatrunculiApp App
        {
            get { return (LatrunculiApp)GetValue(AppProperty); }
            set { SetValue(AppProperty, value); }
        }

        public ChessBoxPosition ActivePosition
        {
            get { return (ChessBoxPosition)GetValue(ActivePositionProperty); }
            set { SetValue(ActivePositionProperty, value); }
        }

        public Move HelpMove
        {
            get { return (Move)GetValue(HelpMoveProperty); }
            set { SetValue(HelpMoveProperty, value); }
        }

        public Board()
        {
            InitializeComponent();
        }

        private void handleBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            var box = sender as Box.Box;
            if (box == null)
            {
                return;
            }

            if (ActivePosition == box.Position)
            {
                ActivePosition = null;
                return;
            }

            var targetState = App.Desk.GetState(box.Position);
            if (targetState == App.HistoryManager.ActualPlayer)
            {
                ActivePosition = box.Position;
                return;
            }

            if (ActivePosition != null && targetState == ChessBoxState.Empty)
            {
                try
                {
                    App.Rules.Move(App.HistoryManager.ActualPlayer, new Move(ActivePosition, box.Position));
                    HelpMove = null;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Chyba při vykonávání tahu");
                };
                ActivePosition = null;
                return;
            }
        }

        private void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void handleLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += handleKeyPress;
        }

        private void handleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                HelpMove = new MiniMaxPlayer(3, App).Turn(App.HistoryManager.ActualPlayer);
            }
        }
    }
}
