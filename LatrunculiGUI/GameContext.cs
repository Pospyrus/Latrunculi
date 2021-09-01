using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Players;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LatrunculiGUI
{
    public class GameContext : DependencyObject, INotifyPropertyChanged
    {
        private CancellationTokenSource helpCts = null;
        private CancellationTokenSource turnCts = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty AppProperty = DependencyProperty.Register(nameof(Latrunculi), typeof(LatrunculiApp), typeof(GameContext), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty HelpMoveProperty = DependencyProperty.Register(nameof(HelpMove), typeof(Move), typeof(GameContext), new FrameworkPropertyMetadata(null));

        public LatrunculiApp Latrunculi
        {
            get { return (LatrunculiApp)GetValue(AppProperty); }
            set { SetValue(AppProperty, value); }
        }

        public Move HelpMove
        {
            get { return (Move)GetValue(HelpMoveProperty); }
            set { SetValue(HelpMoveProperty, value); }
        }

        public GameContext(LatrunculiApp latrunculi)
        {
            Latrunculi = latrunculi;
            latrunculi.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(latrunculi.Desk))
                {
                    helpCts?.Cancel();
                    HelpMove = null;
                }
            };
            TryTurn();
        }

        public void LoadGame()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Latrunculi.SaveGameManager.SaveFolder;
            openFileDialog.Filter = "JSON soubory (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                Latrunculi.SaveGameManager.LoadFromFile(openFileDialog.FileName);
            }
        }

        public void SaveGame()
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.InitialDirectory = Latrunculi.SaveGameManager.SaveFolder;
            openFileDialog.Filter = "JSON soubory (*.json)|*.json";
            openFileDialog.DefaultExt = ".json";
            if (openFileDialog.ShowDialog() == true)
            {
                Latrunculi.SaveGameManager.SaveToFile(openFileDialog.FileName);
            }
        }

        public void GetHelp()
        {
            helpCts?.Cancel();
            var cts = helpCts = new CancellationTokenSource();
            var latrunculi = Latrunculi.CreateClone();
            var task = Task.Run(() =>
            {
                try
                {
                    var move = new MiniMaxPlayer(3, latrunculi).Turn(latrunculi.HistoryManager.ActualPlayer, cts.Token);
                    cts.Token.ThrowIfCancellationRequested();
                    Dispatcher.Invoke(() =>
                    {
                        HelpMove = move;
                    });
                }
                catch
                {
                    Console.WriteLine("Výpočet nejlepšího tahu byl zrušen.");
                }
            });
        }

        public void GetPlayerMove(IPlayer player, Action<Move> callback)
        {
            turnCts?.Cancel();
            var cts = turnCts = new CancellationTokenSource();
            var latrunculi = Latrunculi.CreateClone();
            var task = Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        var move = player.Turn(latrunculi.HistoryManager.ActualPlayer, cts.Token);
                        cts.Token.ThrowIfCancellationRequested();
                        Dispatcher.Invoke(() => callback(move));
                    }));
                }
                catch
                {
                    Console.WriteLine("Výpočet nejlepšího tahu byl zrušen.");
                }
            });
        }

        public void TryTurn()
        {
            var actualPlayer = Latrunculi.ActualPlayer;
            if (actualPlayer != null)
            {
                GetPlayerMove(actualPlayer, move =>
                {
                    if (move != null)
                    {
                        Latrunculi.Rules.Move(Latrunculi.HistoryManager.ActualPlayer, move);
                    }
                    TryTurn();
                });
            }
        }

        public void SetGamePlayerToHuman(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, null);
        }

        public void SetGamePlayerToEasy(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, new RandomPlayer(Latrunculi.Rules));
        }

        public void SetGamePlayerToMedium(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, new MiniMaxPlayer(3, Latrunculi));
        }

        public void SetGamePlayerToExpert(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, new MiniMaxPlayer(4, Latrunculi));
        }

        private void setPlayerType(ChessBoxState playerColor, IPlayer player)
        {
            switch (playerColor)
            {
                case ChessBoxState.Black:
                    Latrunculi.BlackPlayer = player;
                    break;
                case ChessBoxState.White:
                    Latrunculi.WhitePlayer = player;
                    break;
            };
            TryTurn();
        }

        public void Close()
        {
            Application.Current.Shutdown();
        }

        protected void notifyPropertyChanged(string attrName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(attrName));
        }
    }
}
