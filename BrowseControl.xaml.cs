using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Kopiarka.Classes;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack;

namespace Kopiarka
{


    /// <summary>
    /// Interaction logic for BrowseControl.xaml
    /// </summary>
    public partial class BrowseControl : UserControl
    {
        public BrowseControl()
        {
            InitializeComponent();
        }

        public BrowseStyle BrowseStyle
        {
            get { return (BrowseStyle)GetValue(BrowseStyleProperty); }
            set { SetValue(BrowseStyleProperty, value, "BrowseStyle"); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value, "Title"); }
        }

        public string Filters
        {
            get { return (string)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value, "Filters"); }
        }

        public string DefaultExtension
        {
            get { return (string)GetValue(DefaultExtensionProperty); }
            set { SetValue(DefaultExtensionProperty, value, "DefaultExtension"); }
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value, "Path"); }
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value, "Watermark"); }
        }

        public static readonly DependencyProperty BrowseStyleProperty =
          DependencyProperty.Register("BrowseStyle", typeof(BrowseStyle), typeof(BrowseControl));

        public static readonly DependencyProperty TitleProperty =
          DependencyProperty.Register("Title", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty FiltersProperty =
          DependencyProperty.Register("Filters", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty DefaultExtensionProperty =
          DependencyProperty.Register("DefaultExtension", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty PathProperty =
          DependencyProperty.Register("Path", typeof(string), typeof(BrowseControl));

        public static readonly DependencyProperty WatermarkProperty =
          DependencyProperty.Register("Watermark", typeof(string), typeof(BrowseControl));

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetValue(DependencyProperty property, object value, string p = null)
        {
            base.SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        private IEnumerable<CommonFileDialogFilter> ParseFilters()
        {
            if (Filters == null) yield break;

            var split = Filters.Split('|');

            for (var i = 0; i < split.Length - 1; i += 2)
                yield return new CommonFileDialogFilter(split[i], split[i + 1]);
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            using (var d = CreateFileDialog())
            {
                if (d.ShowDialog() == CommonFileDialogResult.Cancel) return;
                Path = d.FileName;
            }
        }

        private CommonFileDialog CreateFileDialog()
        {
            var isDirectory = BrowseStyle.HasFlag(BrowseStyle.Directory);
            var isSave = BrowseStyle.HasFlag(BrowseStyle.Save);
            var isOpen = BrowseStyle.HasFlag(BrowseStyle.Open);

            if (isSave && isDirectory) throw new InvalidOperationException();
            CommonFileDialog d = null;

            if (isSave) d = new CommonSaveFileDialog
            {
                EnsureValidNames = true,
                AlwaysAppendDefaultExtension = true,
                DefaultExtension = DefaultExtension
            };
            if (isOpen) d = new CommonOpenFileDialog
            {
                IsFolderPicker = isDirectory,
                EnsureFileExists = true
            };

            if (d == null) throw new InvalidOperationException();

            d.Title = Title;
            d.NavigateToShortcut = true;
            d.ShowPlacesList = true;

            if (!isDirectory)
                foreach (var f in ParseFilters()) d.Filters.Add(f);

            return d;
        }
    }
}
