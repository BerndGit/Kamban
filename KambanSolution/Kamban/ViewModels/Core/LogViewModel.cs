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

            var CardChanges = Box.Cards.Connect();
            CardChanges.WhereReasonsAre(ListChangeReason.Add)
                .Subscribe(c =>
                {
                    foreach (Change<CardViewModel> ccvm in c)
                    {
                        LogCardChanges(ccvm.Item.Current);
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
                        {
                        LogCardChanges(cvm);
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


        public void LogCardChanges(CardViewModel cvm)
        {
            List<String> notLogging = new List<string>()        {      "Modified", "Order"     };

            cvm.Changing.Subscribe(c =>
            {
                if (notLogging.Contains(c.PropertyName)) return;

                LogEntries.Add(
                            new LogEntry()
                            {
                                Time = DateTime.Now,
                                Property = c.PropertyName,
                                OldValue = cvm.GetType().GetProperty(c.PropertyName).GetValue(cvm, null)?.ToString(),

                                CloumnId = cvm.ColumnDeterminant,
                                RowId = cvm.RowDeterminant,
                                BoardId = cvm.BoardId,
                                CardId = cvm.Id,

                                Source = "Card Changed",
                               

                            }); ; ;


            });


            cvm.Changed.Subscribe(c =>
                 {
                     if (notLogging.Contains(c.PropertyName)) return;

                     var Entry = LogEntries.Where(x => { return (x.Property == c.PropertyName) & (x.CardId == cvm.Id); }).Last();
                     if (Entry!=null)
                     { 
                         Entry.NewValue = cvm.GetType().GetProperty(c.PropertyName).GetValue(cvm, null)?.ToString();
                         Entry.Note = "Property [" + c.PropertyName + "]: " + Entry.OldValue + " -> " + Entry.NewValue;

                         Entry.Cloumn = Box.Columns.Items.Where(x => { return (x.Id == Entry.CloumnId); }).First().Name;
                         Entry.Row = Box.Rows.Items.Where(x => { return (x.Id == Entry.RowId); }).First().Name;
                         Entry.Board = Box.Rows.Items.Where(x => { return (x.Id == Entry.BoardId); }).First().Name;

                     }

                     ;

                 }); 


         

            
               
        }


    }

 

  
    
}
