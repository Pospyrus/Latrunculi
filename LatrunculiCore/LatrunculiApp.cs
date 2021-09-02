using System;
using System.ComponentModel;
using System.IO;
using LatrunculiCore.Desk;
using LatrunculiCore.Exceptions;
using LatrunculiCore.Logger;
using LatrunculiCore.Moves;
using LatrunculiCore.Players;
using LatrunculiCore.SaveGame;

namespace LatrunculiCore
{
    public class LatrunculiApp : INotifyPropertyChanged
    {
        private IPlayer whitePlayer;
        private IPlayer blackPlayer;

        public DeskManager Desk { get; private set; }
        public AllPositions AllPositions { get; private set; }
        public DeskHistoryManager HistoryManager { get; private set; }
        public RulesManager Rules { get; private set; }
        public SaveGameManager SaveGameManager { get; private set; }

        public bool BestMovesDebug = true;
        public bool Debug = false;

        public IPlayer WhitePlayer
        {
            get => whitePlayer;
            set
            {
                whitePlayer = value;
                notifyPropertyChanged(nameof(WhitePlayer));
                if (HistoryManager.ActualPlayer == ChessBoxState.White)
                {
                    notifyPropertyChanged(nameof(ActualPlayer));
                }
            }
        }

        public IPlayer BlackPlayer
        {
            get => blackPlayer;
            set
            {
                blackPlayer = value;
                notifyPropertyChanged(nameof(BlackPlayer));
                if (HistoryManager.ActualPlayer == ChessBoxState.Black)
                {
                    notifyPropertyChanged(nameof(ActualPlayer));
                }
            }
        }

        public IPlayer ActualPlayer => HistoryManager.ActualPlayer == ChessBoxState.Black ? BlackPlayer : WhitePlayer;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsEnded
        {
            get
            {
                try
                {
                    Rules.CheckEndOfGame();
                }
                catch (EndOfGameException)
                {
                    return true;
                }
                return false;
            }
        }

        public string AppDataFolder { get; private set; }

        public LatrunculiApp()
        {
            AppDataFolder = prepareAppDataFolder();
            FileLogger.init(AppDataFolder);
            var deskSize = new DeskSize(8, 7);
            AllPositions = new AllPositions(deskSize);
            Desk = new DeskManager(deskSize);
            Desk.DeskChanged += handleDeskChanged;
            HistoryManager = new DeskHistoryManager();
            HistoryManager.GoingPrev += handleHistoryGoingPrev;
            HistoryManager.GoingBack += handleHistoryGoingBack;
            HistoryManager.PropertyChanged += handleHistoryPropertyChanged;
            Rules = new RulesManager(Desk, AllPositions, HistoryManager);
            Rules.Moved += handleRulesMoved;
            SaveGameManager = new SaveGameManager(this);
            new DeskSpawner(Desk).Spawn();
        }

        public Move Turn()
        {
            var move = ActualPlayer.Turn(this, HistoryManager.ActualPlayer);
            if (move != null)
            {
                Rules.Move(HistoryManager.ActualPlayer, move);
            }
            return move;
        }

        private void handleRulesMoved(object sender, ChangeSet step)
        {
            HistoryManager.Add(step);
        }

        private void handleDeskChanged(object sender, EventArgs e)
        {
            notifyPropertyChanged(nameof(Desk));
        }

        private void handleHistoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HistoryManager.HistoryIndex))
            {
                notifyPropertyChanged(nameof(ActualPlayer));
            }
        }

        private void handleHistoryGoingBack(object sender, ChangeSet changes)
        {
            Desk.RevertStep(changes);
        }

        private void handleHistoryGoingPrev(object sender, ChangeSet changes)
        {
            Desk.DoStep(changes);
        }

        private string prepareAppDataFolder()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Latrunculi";
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            return appDataFolder;
        }

        protected void notifyPropertyChanged(string attrName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(attrName));
        }

        public LatrunculiApp CreateClone()
        {
            var clone = new LatrunculiApp();
            clone.HistoryManager.LoadHistory(HistoryManager.Steps);
            return clone;
        }

        public void Dispose()
        {
            WhitePlayer = null;
            BlackPlayer = null;
            HistoryManager.GoingPrev -= handleHistoryGoingPrev;
            HistoryManager.GoingBack -= handleHistoryGoingBack;
            HistoryManager.PropertyChanged -= handleHistoryPropertyChanged;
            HistoryManager.Dispose();
            HistoryManager = null;
            Rules.Moved -= handleRulesMoved;
            Rules.Dispose();
            Rules = null;
            Desk.DeskChanged -= handleDeskChanged;
            Desk.Dispose();
            Desk = null;
            AllPositions.Dispose();
            AllPositions = null;
            SaveGameManager.Dispose();
            SaveGameManager = null;
        }
    }
}
