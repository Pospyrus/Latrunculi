using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Players;
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

        protected void notifyPropertyChanged(string attrName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(attrName));
        }
    }
}
