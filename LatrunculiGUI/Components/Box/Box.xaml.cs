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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LatrunculiGUI.Components.Box
{
    public enum BoxUIState {
        normal,
        active,
        help
    }

    /// <summary>
    /// Interaction logic for Box.xaml
    /// </summary>
    public partial class Box : UserControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(ChessBoxPosition), typeof(Box), new FrameworkPropertyMetadata(null));

        public ChessBoxPosition Position
        {
            get { return (ChessBoxPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty ChessStateProperty = DependencyProperty.Register(nameof(ChessState), typeof(ChessBoxState), typeof(Box), new FrameworkPropertyMetadata(ChessBoxState.Empty));

        public ChessBoxState ChessState
        {
            get { return (ChessBoxState)GetValue(ChessStateProperty); }
            set { SetValue(ChessStateProperty, value); }
        }

        public static readonly DependencyProperty GuiStateProperty = DependencyProperty.Register(nameof(GuiState), typeof(BoxUIState), typeof(Box), new FrameworkPropertyMetadata(BoxUIState.normal));

        public BoxUIState GuiState
        {
            get { return (BoxUIState)GetValue(GuiStateProperty); }
            set { SetValue(GuiStateProperty, value); }
        }

        public Box()
        {
            InitializeComponent();
        }
    }
}
