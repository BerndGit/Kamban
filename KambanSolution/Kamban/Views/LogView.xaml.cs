using Kamban.ViewModels;
using Kamban.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;
using Ui.Wpf.Common.ViewModels;

namespace Kamban.Views
{
    /// <summary>
    /// Interaktionslogik für LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, IView, IStretchedSizeView
    {
    
        public LogViewModel Log { get; set; }
        public ObservableCollection<ILogEntry> LogEntries { get; }

        public LogView(LogViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;

            Log = (LogViewModel) ViewModel;
            LogEntries = Log.LogEntries;


           // LogGrid.ItemsSource = ;
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
            ViewModel.FullTitle = options.Title;
        }
    }
}
