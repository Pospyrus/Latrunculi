using System;
using System.Collections.Generic;
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
        public readonly DeskManager Desk;
        public readonly AllPositions AllPositions;
        public readonly DeskHistoryManager HistoryManager;
        public readonly RulesManager Rules;
        public readonly SaveGameManager SaveGameManager;

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

        public readonly string AppDataFolder;

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
