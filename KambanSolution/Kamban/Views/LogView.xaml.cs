using Kamban.ViewModels.Core;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;
using Ui.Wpf.Common.ViewModels;
using Microsoft.Win32;


namespace Kamban.Views
{
    /// <summary>
    /// Interaktionslogik für LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, IView, IStretchedSizeView
    {
  
        public LogView(LogViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
            DataContext =  ViewModel;
        }

        public IViewModel ViewModel { get; set; }

        double IStretchedSizeView.StretchedWidth
        {
            get { return 400; }
        }

        double IStretchedSizeView.StretchedHeight
        {
            get { return 400; }
        }


        public void Configure(UiShowOptions options)
        {
             ViewModel.FullTitle = "ABCD: "; //options.Title;

        }

        private void BuNewEntry_Click(object sender, RoutedEventArgs e)
        {
            ((LogViewModel)ViewModel).MakeNewEntry();
        }

        
        

        private void BuExport_Click(object sender, RoutedEventArgs e)
        {
            String FileName;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Comma Separated File (*.csv)|*.csv";
            saveFileDialog.ShowDialog();
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                FileName = saveFileDialog.FileName;
            }
            else
            {
                return;
            }
            String outString = String.Join(";",
                       "Date",
                       "Time",
                       "Row",
                       "Column",
                       "Automatic",
                       "Topic",
                       "Note") +"\r\n";

            
            foreach (LogEntryViewModel entry in ((LogViewModel)ViewModel).FilteredLogEntries)
            {
                String Line = String.Join(";",
                    entry.Time.ToString("yyyy/mm/dd"),
                    entry.Time.ToString("HH:mm:ss.Z"),
                    entry.Row,
                    entry.Column,
                    entry.Automatic,
                    entry.Topic,
                    entry.Note
                    ).Replace("\r\n", @"\r\n");
                outString += Line + "\r\n";
            }

            File.WriteAllText(FileName, outString);
        }

        private void CbTopic_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(ComboBoxItem))
                return;

            try
            {
                ComboBox cb = (ComboBox)sender;
                cb.IsDropDownOpen = true;
            }
            catch { };


        }
    }
}
