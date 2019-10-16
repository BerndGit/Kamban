using DynamicData;
using Kamban.ViewRequests;
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

        public ObservableCollection<ILogEntry> LogEntries { get; set; } = new ObservableCollection<ILogEntry>();
        public BoxViewModel Box;


        public void Initialize(ViewRequest viewRequest)
        {

            Box = ((LogViewRequest)viewRequest).Box;

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



            var CardChanges = Box.Cards.Connect();
            CardChanges.WhereReasonsAre(ListChangeReason.Add)
                .Subscribe(c =>
                {
                    foreach (Change<CardViewModel> ccvm in c)
                    {

                        LogEntries.Add(
                            new LogEntry()
                            {
                                Time = ccvm.Item.Current.Created,
                                CloumnId = ccvm.Item.Current.ColumnDeterminant,
                                RowId = ccvm.Item.Current.RowDeterminant,
                                Source = ccvm.Reason.ToString(),
                                Note = ccvm.Item.Current.Header

                            });
                    }
                });

            CardChanges.WhereReasonsAre(ListChangeReason.AddRange)
                .Subscribe(c =>
                {
                    foreach (Change<CardViewModel> ccvm in c)
                    {
                        foreach (CardViewModel cvm in ccvm.Range.ToList())
                        LogEntries.Add(
                            new LogEntry()
                            {
                                Time = cvm.Created,
                                CloumnId = cvm.ColumnDeterminant,
                                RowId = cvm.RowDeterminant,
                                Source = ccvm.Reason.ToString(),
                                Note = cvm.Header

                            });
                    }
                });


            CardChanges.WhereReasonsAre(ListChangeReason.Remove)
            .Subscribe(c =>
            {
                foreach (Change<CardViewModel> ccvm in c)
                {
                    LogEntries.Add(
                        new LogEntry()
                        {
                            Time = ccvm.Item.Current.Created,
                            CloumnId = ccvm.Item.Current.ColumnDeterminant,
                            RowId = ccvm.Item.Current.RowDeterminant,
                            Source = ccvm.Reason.ToString(),
                            Note = ccvm.Item.Current.Header

                        });
                }
            });

            CardChanges.WhereReasonsAre(ListChangeReason.RemoveRange)
             .Subscribe(c =>
             {
                 foreach (Change<CardViewModel> ccvm in c)
                 {
                     foreach (CardViewModel cvm in ccvm.Range.ToList())
                         LogEntries.Add(
                                         new LogEntry()
                                         {
                                             Time = cvm.Created,
                                             CloumnId = cvm.ColumnDeterminant,
                                             RowId = cvm.RowDeterminant,
                                             Source = ccvm.Reason.ToString(),
                                             Note = cvm.Header

                                         });
                 }
             });

            // throw new NotImplementedException();
        }


    }

  
    
}
