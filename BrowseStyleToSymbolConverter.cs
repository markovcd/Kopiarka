using System;
using System.Globalization;
using System.Windows.Data;

namespace Kopiarka.Classes
{
    [Flags]
    public enum BrowseStyle
    {
        File = 1, Directory = 2, Open = 4, Save = 8
    }

    public class BrowseStyleToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var browseStyle = (BrowseStyle) value;
            if (browseStyle.HasFlag(BrowseStyle.Open)) return "📂";
            if (browseStyle.HasFlag(BrowseStyle.Save)) return "💾";
            return "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals("📂")) return BrowseStyle.Open;
            if (value.Equals("💾")) return BrowseStyle.Save;
            return BrowseStyle.Open;
        }
    }
}
