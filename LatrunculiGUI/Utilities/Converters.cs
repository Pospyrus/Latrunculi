using LatrunculiCore.Desk;
using System;
using System.Globalization;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Windows.Media;
using LatrunculiGUI.Components.Box;
using System.Linq;
using System.Collections.Generic;

namespace LatrunculiGUI.Utilities
{
    public static class IListExtensions
    {
        public static TReturn GetValueByIndex<TReturn>(this IList source, int index, TReturn defaultValue = default)
        {
            if (source == null || index >= source.Count)
            {
                return defaultValue;
            }
            try
            {
                return (TReturn)(dynamic)(source[index]);
            }
            catch
            {
                return default;
            }
        }
    }

    public class BoxFillColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var position = values.GetValueByIndex<ChessBoxPosition>(0);
            var guiState = values.GetValueByIndex<BoxUIState>(1);
            var isBlack = position == null || position.Index % 2 == 0;
            return guiState switch
            {
                BoxUIState.normal => isBlack ? Brushes.SaddleBrown : Brushes.Gray,
                BoxUIState.active => isBlack ? Brushes.SandyBrown : Brushes.LightGray,
                BoxUIState.help => isBlack ? Brushes.DarkBlue : Brushes.LightBlue,
                _ => Brushes.Transparent
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoxSymbolFillColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = value as ChessBoxState?;
            return state switch
            {
                ChessBoxState.Black => Brushes.Black,
                ChessBoxState.White => Brushes.White,
                _ => Brushes.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoxPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var position = values.GetValueByIndex<int>(0);
            var boxSize = values.GetValueByIndex<double>(1);
            return (double)(position * boxSize);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoxSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var boardWidth = values.GetValueByIndex<double>(0);
            var boardHeight = values.GetValueByIndex<double>(1);
            var deskSize = values.GetValueByIndex<DeskSize>(2);

            if (deskSize == null)
            {
                return 0.0;
            }

            var boxWidthMax = boardWidth / (deskSize.Width + 0.5);
            var boxHeightMax = boardHeight / (deskSize.Height + 0.5);
            return Math.Ceiling(Math.Min(boxWidthMax, boxHeightMax));

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DeskSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var boxSize = values.GetValueByIndex<double>(0);
            var deskSize = values.GetValueByIndex<DeskSize>(1);
            return new
            {
                Width = (deskSize?.Width ?? 0) * boxSize,
                Height = (deskSize?.Height ?? 0) * boxSize
            };

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoxGuiStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var position = values.GetValueByIndex<ChessBoxPosition>(0);
            var activePosition = values.GetValueByIndex<ChessBoxPosition>(1);
            var helpMove = values.GetValueByIndex<Move>(2);
            if (activePosition == position)
            {
                return BoxUIState.active;
            }
            if (helpMove?.From == position || helpMove?.To == position)
            {
                return BoxUIState.help;
            }
            return BoxUIState.normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BindingProperty : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(BindingProperty), new FrameworkPropertyMetadata(15.10));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }

    public class BoxStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var desk = values.GetValueByIndex<DeskManager>(0);
            var position = values.GetValueByIndex<ChessBoxPosition>(1);
            if (desk == null || position == null)
            {
                return ChessBoxState.Empty;
            }
            return desk.GetState(position);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumbersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value as int?;
            return Enumerable.Range(0, count.GetValueOrDefault());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = value as int?;
            return (char)('A' + number.GetValueOrDefault());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LetterFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boxSize = (value as double?).GetValueOrDefault();
            return Math.Max(1.0, boxSize / 4.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoardWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var actualHeight = values.GetValueByIndex<double>(0);
            var deskSize = values.GetValueByIndex<DeskSize>(1);
            if (deskSize == null)
            {
                return actualHeight;
            }
            var ratio = (deskSize.Width + 0.5) / (deskSize.Height + 0.5);
            return actualHeight * ratio;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumeratorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable<object>;
            return enumerable.Select((item, index) => new { Index = index, Value = item });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IncrementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = value as int?;
            return number.GetValueOrDefault() + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
