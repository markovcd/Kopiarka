using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Kopiarka.Classes;
using Microsoft;
using System.ComponentModel;

namespace Kopiarka
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SheetCopier copier;

        public static string FilesPath { get; private set; } 
        public static IList<string> FileList { get; private set; } 

        public MainWindow()
        {
            InitializeComponent();
            
            FilesPath = AppDomain.CurrentDomain.BaseDirectory + "\\pliki\\";
            FileList = Directory.EnumerateFiles(FilesPath, "*.txt", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToList();
            
            if (!FileList.Any())
            {
                MessageBox.Show("Brak pliku konfiguracyjnego.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            configuration.ItemsSource = FileList;
            configuration.SelectedIndex = FileList.IndexOf("wszystko.txt") >= 0 ? FileList.IndexOf("wszystko.txt") : 0;
        }

		private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (!dateFrom.SelectedDate.HasValue || !dateTo.SelectedDate.HasValue || !destination.Path.Trim().Any()) return;
			
            cancel.Visibility = Visibility.Visible;
        	copy.Visibility = Visibility.Hidden;
            
            copier = new SheetCopier(dateFrom.SelectedDate.Value, dateTo.SelectedDate.Value, FilesPath, configuration.Text, 
                                         destination.Path, updateDates.IsChecked.Value, overwrite.IsChecked.Value);
            
            copier.ProgressChanged += delegate(object sender2, ProgressChangedEventArgs e2) 
            { 
            	Title = String.Format("Kopiarka Arkuszy - {0}%", e2.ProgressPercentage); 
            };           
            
            copier.RunWorkerCompleted += delegate 
            {
            	Title = "Kopiarka Arkuszy";
            	cancel.Visibility = Visibility.Hidden;
            	copy.Visibility = Visibility.Visible;
            	copier.Dispose();
            };

            copier.RunWorkerAsync();
        }
		
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
        	if (copier != null) copier.CancelAsync();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (copier != null) copier.CancelAsync();
        }
    }
}
