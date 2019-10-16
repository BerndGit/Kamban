using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;
using Ui.Wpf.Common.ViewModels;

namespace Kamban.ViewModels.Core
{
    public partial class LogViewModel : ViewModelBase, IInitializableViewModel
    {
        public const string LogViewId = "KambanLogView";

        public ObservableCollection<ILogEntry> LogEntries { get; set; }  = new ObservableCollection<ILogEntry>();


        public void Initialize(ViewRequest viewRequest)
        {
            LogEntries.Add(
                new LogEntry()
                {
                    Time = new DateTime(),
                    CloumnId = 1,
                    Note = "Hallo"
                });

            LogEntries.Add(
                new LogEntry()
                {
                    Time = new DateTime(),
                    CloumnId = 1,
                    Note = "XXXX"
                });

            LogEntries.Add(
                new LogEntry()
                {
                    Time = new DateTime(),
                    CloumnId = 2,
                    Note = "Du"
                });

            // throw new NotImplementedException();
        }


    }

  
    
}
