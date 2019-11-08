using DynamicData;
using Kamban.Repository;
using Kamban.Repository.Models;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamban.ViewModels.Core
{
    public partial class BoxViewModel 
    {

        [Reactive] public int TotalLogEntries { get; set; }
        [Reactive] public SourceList<LogEntryViewModel> LogEntries { get; set; } = new SourceList<LogEntryViewModel>();

        ISaveRepository Repo;


        private void Repo_CreateOrUpdateLogEntry(ISaveRepository repo, LogEntryViewModel Entry)
        {
            var en = mapper.Map<LogEntryViewModel, LogEntry>(Entry);
            en = repo.CreateOrUpdateLogEntry(en).Result;

            Entry.Id = en.Id;
        }

        private void Entry_PropertyChanged(object Entry, System.ComponentModel.PropertyChangedEventArgs e)
        {

            Repo_CreateOrUpdateLogEntry(Repo, (LogEntryViewModel) Entry);
            

        }

        public void InitializeLogger(ISaveRepository repo)
        {
            /////////////////////////////////////////
            /// Mirror LogEntries to repo
            /////////////////////////////////////////

            LogEntries
                 .Connect()
                 .WhereReasonsAre(ListChangeReason.Add )
                 .Subscribe(ch =>
                 {
                     foreach (Change<LogEntryViewModel> c in ch)
                     {
                         LogEntryViewModel Entry = c.Item.Current;
                         if (Entry != null)
                         {
                             Repo_CreateOrUpdateLogEntry(repo, Entry);
                             Entry.PropertyChanged += Entry_PropertyChanged;
                         }
                         if (c.Range != null)
                         {
                             foreach (LogEntryViewModel REntry in c.Range.ToList())
                             {
                                 Repo_CreateOrUpdateLogEntry(repo, REntry);
                                 REntry.PropertyChanged += Entry_PropertyChanged;
                             }
                         }
                     }
                 });

            LogEntries
                 .Connect()
                 .WhereReasonsAre(ListChangeReason.AddRange)
                 .Subscribe(ch =>
                 {
                     foreach (Change<LogEntryViewModel> c in ch)
                     {
                         LogEntryViewModel Entry = c.Item.Current;
                         if (Entry != null)
                         {
                             Entry.PropertyChanged += Entry_PropertyChanged;
                         }
                         if (c.Range != null)
                         {
                             foreach (LogEntryViewModel REntry in c.Range.ToList())
                             {
                                 REntry.PropertyChanged += Entry_PropertyChanged;
                             }
                         }
                     }
                 });


            LogEntries
                 .Connect()
                 .WhereReasonsAre(ListChangeReason.Remove)
                 .Subscribe(ch =>
                 {
                     foreach (Change<LogEntryViewModel> c in ch)
                     {
                         LogEntryViewModel Entry = c.Item.Current;
                         if (Entry != null)
                         {
                             Repo_CreateOrUpdateLogEntry(repo, Entry);
                             repo.RemoveLogEntry(Entry.Id);
                         }
                         if (c.Range != null)
                         {
                             foreach (LogEntryViewModel REntry in c.Range.ToList())
                             {
                                 repo.RemoveLogEntry(REntry.Id);
                             }
                         }
                     }
                 });

            /////////////////////////////////////////
            /// Log Cards
            /////////////////////////////////////////

            var CardChanges = Cards.Connect();
            CardChanges.WhereReasonsAre(ListChangeReason.Add)
                .Subscribe(c =>
                {
                    foreach (Change<CardViewModel> ccvm in c)
                    {
                        LogCardChanges(ccvm.Item.Current, repo);

                        LogEntryViewModel Entry = new LogEntryViewModel()
                        {
                            Time = DateTime.Now,
                            ColumnId = ccvm.Item.Current.ColumnDeterminant,
                                                     
                            RowId = ccvm.Item.Current.RowDeterminant,
                            CardId = ccvm.Item.Current.Id,
                            BoardId = ccvm.Item.Current.BoardId,

                            Topic = ccvm.Reason.ToString(),
                            Note = ccvm.Item.Current.Header
                        };

                        Entry.Note = "Add Card #" + ccvm.Item.Current.Id.ToString() + ": " + ccvm.Item.Current.Header + "\r\n";

                        Entry.Column = Columns.Items.Where(x => { return (x.Id == Entry.ColumnId); }).First().Name;
                        Entry.Row = Rows.Items.Where(x => { return (x.Id == Entry.RowId); }).First().Name;
                        Entry.Board = Rows.Items.Where(x => { return (x.Id == Entry.BoardId); }).First().Name;

                        LogEntries.Add(Entry);

                    };
                });

            CardChanges.WhereReasonsAre(ListChangeReason.AddRange)
                .Subscribe(c =>
                {
                    foreach (Change<CardViewModel> ccvm in c)
                    {
                        foreach (CardViewModel cvm in ccvm.Range.ToList())
                        {
                            LogCardChanges(cvm, repo); // Add Observer

                            // Don't generate Log Entry, since AddRange is issued only at loading of Box.
                        }
                    }
                });


            CardChanges.WhereReasonsAre(ListChangeReason.Remove)
            .Subscribe(c =>
            {
                foreach (Change<CardViewModel> ccvm in c)
                {
                    LogEntryViewModel Entry = new LogEntryViewModel()
                    {
                        Time = DateTime.Now,
                        ColumnId = ccvm.Item.Current.ColumnDeterminant,
                        RowId = ccvm.Item.Current.RowDeterminant,
                        BoardId = ccvm.Item.Current.BoardId,
                        Topic = ccvm.Reason.ToString(),
                        Note = ccvm.Item.Current.Header
                    };

                    Entry.Note = "Remove Card #" + ccvm.Item.Current.Id.ToString() + ": " + ccvm.Item.Current.Header + "\r\n";

                    Entry.Column = Columns.Items.Where(x => { return (x.Id == Entry.ColumnId); }).First().Name;
                    Entry.Row = Rows.Items.Where(x => { return (x.Id == Entry.RowId); }).First().Name;
                    Entry.Board = Rows.Items.Where(x => { return (x.Id == Entry.BoardId); }).First().Name;

                    LogEntries.Add(Entry);

                }
            });

            CardChanges.WhereReasonsAre(ListChangeReason.RemoveRange)
             .Subscribe(c =>
             {
                 foreach (Change<CardViewModel> ccvm in c)
                 {
                     foreach (CardViewModel cvm in ccvm.Range.ToList())
                     {
                         LogEntryViewModel Entry = new LogEntryViewModel()
                         {
                             Time = DateTime.Now,
                             ColumnId = cvm.ColumnDeterminant,
                             RowId = cvm.RowDeterminant,
                             BoardId = cvm.BoardId,
                             Topic = ccvm.Reason.ToString()
                         };

                         Entry.Note = "Remove Card #" + ccvm.Item.Current.Id.ToString() + ": " + ccvm.Item.Current.Header + "\r\n";

                         Entry.Column = Columns.Items.Where(x => { return (x.Id == Entry.ColumnId); }).First().Name;
                         Entry.Row = Rows.Items.Where(x => { return (x.Id == Entry.RowId); }).First().Name;
                         Entry.Board = Rows.Items.Where(x => { return (x.Id == Entry.BoardId); }).First().Name;

                         LogEntries.Add(Entry);
                     }
                 }
             });
        }


        public void LogCardChanges(CardViewModel cvm, ISaveRepository repo)
        {
            List<String> notLogging = new List<string>() { "Modified", "Order", "ShowDescription" };

            cvm.Changing.Subscribe(c =>
            {
                if (notLogging.Contains(c.PropertyName)) return;

                LogEntries.Add(
                            new LogEntryViewModel()
                            {
                                CardId = cvm.Id,
                                Property = c.PropertyName,
                                OldValue = cvm.GetType().GetProperty(c.PropertyName).GetValue(cvm, null)?.ToString(),
                            });
            });


            cvm.Changed.Subscribe(c =>
            {
                if (notLogging.Contains(c.PropertyName)) return;

                var Entry = LogEntries.Items.Where(x => { return (x.Property == c.PropertyName) & (x.CardId == cvm.Id); }).Last();

                bool noEntryfound = (Entry == null);
                if (noEntryfound) // Should not happen
                {
                    Entry = new LogEntryViewModel()
                    {
                        Property = c.PropertyName,
                        OldValue = "",
                    };
                };

                Entry.Time = DateTime.Now;
                Entry.Topic = "Card Changed";

                Entry.ColumnId = cvm.ColumnDeterminant;
                Entry.RowId = cvm.RowDeterminant;
                Entry.BoardId = cvm.BoardId;
                Entry.CardId = cvm.Id;

                Entry.Column = Columns.Items.Where(x => { return (x.Id == Entry.ColumnId); }).First().Name;
                Entry.Row = Rows.Items.Where(x => { return (x.Id == Entry.RowId); }).First().Name;
                Entry.Board = Rows.Items.Where(x => { return (x.Id == Entry.BoardId); }).First().Name;

                Entry.NewValue = cvm.GetType().GetProperty(c.PropertyName).GetValue(cvm, null)?.ToString();
                Entry.Note = "Card #" + cvm.Id.ToString() +": " + cvm.Header + "\r\n" +
                            c.PropertyName + ": " + Entry.OldValue + " -> " + Entry.NewValue;

                if (Entry.Property== "ColumnDeterminant")
                {
                    Entry.Note = "Card #" + cvm.Id.ToString() + ": " + cvm.Header + "\r\n" +
                            "Column: " + Columns.Items.Where(x => { return (x.Id.ToString() == Entry.OldValue); }).First().Name  + " -> " + Entry.Column;
                }

                if (Entry.Property == "RowDeterminant")
                {
                    Entry.Note = "Card #" + cvm.Id.ToString() + ": " + cvm.Header + "\r\n" +
                            "Row: " + Rows.Items.Where(x => { return (x.Id.ToString() == Entry.OldValue); }).First().Name + " -> " + Entry.Row;
                }



                if (noEntryfound)
                {
                    LogEntries.Add(Entry);
                }

            });
        }
    }
}

