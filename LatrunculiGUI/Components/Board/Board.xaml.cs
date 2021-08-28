using LatrunculiCore;
using LatrunculiCore.Desk;
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
namespace LatrunculiGUI.Components.Board
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public static readonly DependencyProperty AppProperty = DependencyProperty.Register(nameof(App), typeof(LatrunculiApp), typeof(Board), new FrameworkPropertyMetadata(null));

        public LatrunculiApp App
        {
            get { return (LatrunculiApp)GetValue(AppProperty); }
            set { SetValue(AppProperty, value); }
        }

        public int BoxSize { get; set; } = 50;

        public Board()
        {
            InitializeComponent();
        }
    }
}
