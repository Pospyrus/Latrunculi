using System.Windows;

namespace LatrunculiGUI.Utilities
{
    public class BindingProperty : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(BindingProperty), new FrameworkPropertyMetadata(10.0));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
