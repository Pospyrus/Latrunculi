using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using LatrunculiCore.Desk;
using LatrunculiCore.Exceptions;
using LatrunculiCore.Logger;
using LatrunculiCore.Moves;
using LatrunculiCore.Players;
using LatrunculiCore.SaveGame;

namespace LatrunculiCore
{
    public class LatrunculiApp
    {
        public DeskManager Desk { get; private set; }
        public AllPositions AllPositions { get; private set; }
        public DeskHistoryManager HistoryManager { get; private set; }
        public RulesManager Rules { get; private set; }
        public SaveGameManager SaveGameManager { get; private set; }

        public bool BestMovesDebug = true;
        public bool Debug = false;

        public IPlayer WhitePlayer;
        public IPlayer BlackPlayer;

        public IPlayer ActualPlayer => HistoryManager.ActualPlayer == ChessBoxState.Black ? BlackPlayer : WhitePlayer;

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
            HistoryManager = new DeskHistoryManager();
            HistoryManager.GoingPrev += (_, changes) => Desk.DoStep(changes);
            HistoryManager.GoingBack += (_, changes) => Desk.RevertStep(changes);
            Rules = new RulesManager(Desk, AllPositions, HistoryManager);
            Rules.Moved += (_, step) => HistoryManager.Add(step);
            SaveGameManager = new SaveGameManager(this);
            new DeskSpawner(Desk).Spawn();
        }

        public Move Turn()
        {
            var move = this.ActualPlayer.Turn(HistoryManager.ActualPlayer);
            if (move != null)
            {
                Rules.Move(HistoryManager.ActualPlayer, move);
            }
            return move;
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
    }
}
