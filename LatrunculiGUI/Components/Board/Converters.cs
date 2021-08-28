using LatrunculiCore.Desk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Windows.Media;

namespace LatrunculiGUI.Components.Board
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

    public class BoxFillColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var position = (ChessBoxPosition)value;
            return position == null ? Brushes.Transparent :
                position.Index % 2 == 0 ? Brushes.Brown :
                Brushes.Gray;
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
            var boardWidth = values.GetValueByIndex<double>(1);
            var boardHeight = values.GetValueByIndex<double>(2);
            var boxSize = BoxSizeConverter.GetBoxSize(boardWidth, boardHeight);
            return (double)(position * boxSize);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoxSizeConverter : IMultiValueConverter
    {
        public static double GetBoxSize(double boardWidth, double boardHeight)
        {
            var boardMinSize = Math.Min(boardWidth, boardHeight);
            return Math.Ceiling(boardMinSize / 7);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var boardWidth = values.GetValueByIndex<int>(0);
            var boardHeight = values.GetValueByIndex<int>(1);
            return GetBoxSize(boardWidth, boardHeight);

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
            var boardWidth = values.GetValueByIndex<double>(0);
            var boardHeight = values.GetValueByIndex<double>(1);
            return Math.Min(boardWidth, boardHeight);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
