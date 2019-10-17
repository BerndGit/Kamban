using DynamicData;
using Kamban.ViewRequests;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public ObservableCollection<ILogEntry> LogEntries = new ObservableCollection<ILogEntry>();
        public BoxViewModel Box;


        public void Initialize(ViewRequest viewRequest)
        {
            Box = ((LogViewRequest)viewRequest).Box;

            this.LogEntries.Clear();
            Box.LogEntries.ToList().ForEach(ent => this.LogEntries.Add(ent));

            Box.LogEntries.CollectionChanged += BoxLogChanged;
            
        }

        private void BoxLogChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems!=null)
            {
                foreach (ILogEntry ent in e.NewItems)
                {
                    this.LogEntries.Add(ent);
                };
            }

            if (e.OldItems != null)
            {
                foreach (ILogEntry ent in e.OldItems)
                {
                    this.LogEntries.Remove(ent);
                };
            }

 
        }
    }

 

  
    
}
