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

namespace LatrunculiGUI
{
    public class GameContext : DependencyObject, INotifyPropertyChanged
    {
        private Task<Move> helpTask;

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
                    Dispatcher.Invoke(() => HelpMove = null);
                }
            };
        }

        public void GetHelp()
        {
            var latrunculi = Latrunculi;
            latrunculi.IsCalculatingHelp = true;
            var task = helpTask = Task.Run(() => new MiniMaxPlayer(3, latrunculi).Turn(latrunculi.HistoryManager.ActualPlayer));
            task.ContinueWith(move =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (helpTask == task)
                    {
                        HelpMove = move.Result;
                        latrunculi.IsCalculatingHelp = false;
                    }
                });
            });
        }

        protected void notifyPropertyChanged(string attrName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(attrName));
        }
    }
}
