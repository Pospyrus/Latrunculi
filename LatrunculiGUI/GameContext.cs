using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Exceptions;
using LatrunculiCore.Players;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace LatrunculiGUI
{
    public class GameContext : DependencyObject, INotifyPropertyChanged
    {
        private CancellationTokenSource helpCts = null;
        private CancellationTokenSource turnCts = null;
        private BackgroundWorker helpWorker;
        private BackgroundWorker turnWorker;

        private bool isTurnPending = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty AppProperty = DependencyProperty.Register(nameof(Latrunculi), typeof(LatrunculiApp), typeof(GameContext), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty HelpMoveProperty = DependencyProperty.Register(nameof(HelpMove), typeof(Move), typeof(GameContext), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(string), typeof(GameContext), new FrameworkPropertyMetadata(null));

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

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public GameContext(LatrunculiApp latrunculi)
        {
            Latrunculi = latrunculi;
            latrunculi.PropertyChanged += (_, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(latrunculi.Desk):
                        helpCts?.Cancel();
                        HelpMove = null;
                        break;
                    case nameof(latrunculi.ActualPlayer):
                        turnCts?.Cancel();
                        helpCts?.Cancel();
                        TryTurn();
                        break;
                };
            };
            latrunculi.HistoryManager.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(latrunculi.HistoryManager.IsLastIndexSelected))
                {
                    if (latrunculi.HistoryManager.IsLastIndexSelected && !latrunculi.IsEnded)
                    {
                        TryTurn();
                    }
                    else
                    {
                        turnCts?.Cancel();
                        helpCts?.Cancel();
                    }
                }
            };
            turnWorker = createMoveWorker(handlePlayerTurn);
            helpWorker = createMoveWorker(handleHelped);
            ActualizeStatus();
            TryTurn();
        }

        private void handleHelped(Move move)
        {
            HelpMove = move;
            ActualizeStatus();
        }

        public void LoadGame()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Latrunculi.SaveGameManager.SaveFolder;
            openFileDialog.Filter = "JSON soubory (*.json)|*.json";
            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }
            if (!Latrunculi.SaveGameManager.LoadFromFile(openFileDialog.FileName))
            {
                MessageBox.Show("Hru se nepovedlo korektně načíst", "Chyba při načítání hry");
            }
        }

        public void SaveGame()
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.InitialDirectory = Latrunculi.SaveGameManager.SaveFolder;
            openFileDialog.Filter = "JSON soubory (*.json)|*.json";
            openFileDialog.DefaultExt = ".json";
            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }
            if (!Latrunculi.SaveGameManager.SaveToFile(openFileDialog.FileName))
            {
                MessageBox.Show("Hru se nepovedlo uložit", "Chyba při ukládání hry");
            }
        }

        public void GetHelp()
        {
            if (Latrunculi.ActualPlayer != null)
            {
                return;
            }
            helpCts?.Cancel();
            var cts = helpCts = new CancellationTokenSource();
            GetPlayerMove(helpWorker, new MiniMaxPlayer(3), cts.Token);
            ActualizeStatus();
        }

        public void GetPlayerMove(BackgroundWorker worker, IPlayer player, CancellationToken ct = default)
        {
            var latrunculi = Latrunculi.CreateClone();
            worker.RunWorkerAsync(new TurnWorkerParams
            {
                Latrunculi = latrunculi,
                Player = player,
                Token = ct
            });
        }

        private class TurnWorkerParams
        {
            public CancellationToken Token { get; set; } = default;
            public IPlayer Player { get; set; }
            public LatrunculiApp Latrunculi { get; set; }
        }

        private BackgroundWorker createMoveWorker(Action<Move> callback)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                var argument = e.Argument as TurnWorkerParams;
                try
                {
                    var move = argument.Player.Turn(argument.Latrunculi, argument.Latrunculi.HistoryManager.ActualPlayer, argument.Token);
                    e.Result = move.CreateClone();
                    Thread.Sleep(500);
                    argument.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    e.Result = null;
                }
                finally
                {
                    argument.Latrunculi?.Dispose();
                }
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                try
                {
                    callback(e.Result as Move);
                }
                finally
                {
                    worker.Dispose();
                }
            };

            return worker;
        }

        public void TryTurn()
        {
            ActualizeStatus();
            if (isTurnPending || !Latrunculi.HistoryManager.IsLastIndexSelected)
            {
                return;
            }

            try
            {
                Latrunculi.Rules.CheckEndOfGame();
            }
            catch (EndOfGameException e)
            {
                MessageBox.Show(e.Message, "Hra skončila");
                ActualizeStatus();
                return;
            }

            var actualPlayer = Latrunculi.ActualPlayer;
            if (actualPlayer != null)
            {
                isTurnPending = true;
                helpCts?.Cancel();
                turnCts?.Cancel();
                var cts = turnCts = new CancellationTokenSource();
                GetPlayerMove(turnWorker, actualPlayer, cts.Token);
                ActualizeStatus();
            }
        }

        private void handlePlayerTurn(Move move)
        {
            if (move != null)
            {
                Latrunculi.Rules.Move(Latrunculi.HistoryManager.ActualPlayer, move);
            }
            isTurnPending = false;
            ActualizeStatus();
            TryTurn();
        }

        public void SetGamePlayerToHuman(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, null);
        }

        public void SetGamePlayerToEasy(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, new RandomPlayer());
        }

        public void SetGamePlayerToMedium(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, new MiniMaxPlayer(2));
        }

        public void SetGamePlayerToExpert(ChessBoxState playerColor)
        {
            setPlayerType(playerColor, new MiniMaxPlayer(3));
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

        public void ActualizeStatus(string message = null)
        {
            if (message != null)
            {
                Status = message;
                return;
            }

            try
            {
                Latrunculi.Rules.CheckEndOfGame();
            }
            catch (EndOfGameException e)
            {
                Status = e.Message;
                return;
            }

            if (!Latrunculi.HistoryManager.IsLastIndexSelected && Latrunculi.ActualPlayer != null)
            {
                Status = $"Proházení historie tahů... Pro pokračování běžte na konec historie.";
                return;
            }

            var actualPlayer = getPlayerName(Latrunculi.HistoryManager.ActualPlayer);

            if (Latrunculi.ActualPlayer != null)
            {
                Status = $"Hraje {actualPlayer} počítačový hráč... Počítám tah...";
                return;
            }

            if (helpWorker.IsBusy)
            {
                Status = $"Hraje {actualPlayer} hráč. Získávám nápovědu nejlepšího tahu...";
                return;
            }

            Status = $"Hraje {actualPlayer} hráč.";
        }

        public void Close()
        {
            Application.Current.Shutdown();
        }

        private string getPlayerName(ChessBoxState player)
        {
            return player == ChessBoxState.Black ? "černý" : "bílý";
        }

        protected void notifyPropertyChanged(string attrName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(attrName));
        }
    }
}
