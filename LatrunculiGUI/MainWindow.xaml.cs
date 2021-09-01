using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Players;
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

namespace LatrunculiGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameContext Game { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Game = new GameContext(new LatrunculiApp());
            DataContext = Game;
        }

        private void handleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                Game.GetHelp();
            }
        }

        private void handleNovaHra(object sender, RoutedEventArgs e)
        {
            Game.Latrunculi.HistoryManager.NewGame();
        }

        private void handleNacistHru(object sender, RoutedEventArgs e)
        {
            Game.LoadGame();
        }

        private void handleUlozitHru(object sender, RoutedEventArgs e)
        {
            Game.SaveGame();
        }

        private void handleUkoncit(object sender, RoutedEventArgs e)
        {
            Game.Close();
        }

        private void handleZiskatNapovedu(object sender, RoutedEventArgs e)
        {
            Game.GetHelp();
        }

        private void handleBilyHracClovek(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToHuman(ChessBoxState.White);
        }

        private void handleBilyHracZacatecnik(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToEasy(ChessBoxState.White);
        }

        private void handleBilyHracPokrocily(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToExpert(ChessBoxState.White);
        }

        private void handleBilyHracExpert(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToHuman(ChessBoxState.White);
        }

        private void handleCernyHracClovek(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToHuman(ChessBoxState.Black);
        }

        private void handleCernyHracZacatecnik(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToEasy(ChessBoxState.Black);
        }

        private void handleCernyHracPokrocily(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToExpert(ChessBoxState.Black);
        }

        private void handleCernyHracExpert(object sender, RoutedEventArgs e)
        {
            Game.SetGamePlayerToHuman(ChessBoxState.Black);
        }
    }
}
