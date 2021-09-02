using LatrunculiCore;
using LatrunculiCore.Desk;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace LatrunculiGUI.Components.Board
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game), typeof(GameContext), typeof(Board), new FrameworkPropertyMetadata(null, handleGameChanged));

        private static void handleGameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = d as Board;
            if (board == null)
            {
                return;
            }

            if (e.OldValue is GameContext gameOld)
            {
                gameOld.Latrunculi.PropertyChanged -= board.latrunculiPropertyChanged;
            }

            if (e.NewValue is GameContext game)
            {
                game.Latrunculi.PropertyChanged += board.latrunculiPropertyChanged;
            }
        }

        private void latrunculiPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LatrunculiApp.ActualPlayer) && sender is LatrunculiApp latrunculi && latrunculi.ActualPlayer != null)
            {
                ActivePosition = null;
            }
        }

        public static readonly DependencyProperty ActivePositionProperty = DependencyProperty.Register(nameof(ActivePosition), typeof(ChessBoxPosition), typeof(Board), new FrameworkPropertyMetadata(null));

        public GameContext Game
        {
            get { return (GameContext)GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }

        public ChessBoxPosition ActivePosition
        {
            get { return (ChessBoxPosition)GetValue(ActivePositionProperty); }
            set { SetValue(ActivePositionProperty, value); }
        }

        public Board()
        {
            InitializeComponent();
        }

        private void handleBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Game.Latrunculi.ActualPlayer != null)
            {
                return;
            }

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

            var targetState = Game.Latrunculi.Desk.GetState(box.Position);
            if (targetState == Game.Latrunculi.HistoryManager.ActualPlayer)
            {
                ActivePosition = box.Position;
                return;
            }

            if (ActivePosition != null && targetState == ChessBoxState.Empty)
            {
                try
                {
                    Game.Latrunculi.Rules.Move(Game.Latrunculi.HistoryManager.ActualPlayer, new Move(ActivePosition, box.Position));
                    Game.TryTurn();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Chyba při vykonávání tahu");
                };
                ActivePosition = null;
                return;
            }
        }
    }
}
