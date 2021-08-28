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
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game), typeof(GameContext), typeof(Board), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ActivePositionProperty = DependencyProperty.Register(nameof(ActivePosition), typeof(ChessBoxPosition), typeof(Board), new FrameworkPropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

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
            if (Game.Latrunculi.IsCalculatingHelp)
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
    }
}
