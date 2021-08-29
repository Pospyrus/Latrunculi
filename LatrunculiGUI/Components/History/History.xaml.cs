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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LatrunculiGUI.Components.History
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game), typeof(GameContext), typeof(History), new FrameworkPropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        public GameContext Game
        {
            get { return (GameContext)GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }

        public History()
        {
            InitializeComponent();
        }

        private void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
