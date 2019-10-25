using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DynamicData;
using Kamban.Extensions;
using Kamban.Repository;
using Kamban.Repository.Models;
using Monik.Common;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kamban.ViewModels.Core
{
    public class BoxViewModel : ReactiveObject
    {
        [Reactive] public string Uri { get; set; }
        [Reactive] public bool Loaded { get; set; }

        [Reactive] public string Title { get; set; }
        [Reactive] public int TotalTickets { get; set; }
        [Reactive] public int TotalLogEntries { get; set; }
        [Reactive] public string BoardList { get; set; }

        [Reactive] public SourceList<ColumnViewModel> Columns { get; set; }
        [Reactive] public SourceList<RowViewModel> Rows { get; set; }
        [Reactive] public SourceList<BoardViewModel> Boards { get; set; }

        [Reactive] public SourceList<CardViewModel> Cards { get; set; }

        [Reactive] public SourceList<LogEntryViewModel> LogEntries { get; set; } = new SourceList<LogEntryViewModel>();

        ISaveRepository Repo;

        private readonly IMonik mon;
        private readonly IMapper mapper;

        public BoxViewModel(IMonik monik, IMapper mapper)
        {
            this.mon = monik;
            this.mapper = mapper;

            Columns = new SourceList<ColumnViewModel>();
            Rows = new SourceList<RowViewModel>();

            Boards = new SourceList<BoardViewModel>();
            Cards = new SourceList<CardViewModel>();

            Cards
                .Connect()
                .AutoRefresh()
                .Subscribe(x => TotalTickets = Cards.Count);

            Boards
                .Connect()
                .AutoRefresh()
                .Subscribe(bvm =>
                {
                    var lst = Boards.Items.Select(x => x.Name).ToList();
                    var str = string.Join(",", lst);
                    BoardList = str;
                });
        }

        public void Connect(ISaveRepository repo)
        {
            Repo = repo;

            ///////////////////
            // Boards AutoSaver
            ///////////////////
            var boardsChanges = Boards.Connect().Publish();
            SubscribeChanged(boardsChanges,
                bvm =>
                {
                    mon.LogicVerbose($"Box.Boards.ItemChanged {bvm.Id}::{bvm.Name}::{bvm.Modified}");
                    var bi = mapper.Map<BoardViewModel, Board>(bvm);
                    repo.CreateOrUpdateBoard(bi).Wait();
                });

            SubscribeAdded(boardsChanges,
                bvm =>
                {
                    mon.LogicVerbose($"Box.Boards.Add {bvm.Id}::{bvm.Name}");

                    var bi = mapper.Map<BoardViewModel, Board>(bvm);
                    bi = repo.CreateOrUpdateBoard(bi).Result;

                    bvm.Id = bi.Id;
                });

            SubscribeRemoved(boardsChanges,
                bvm =>
                {
                    mon.LogicVerbose($"Box.Boards.Remove {bvm.Id}::{bvm.Name}");

                    repo.DeleteBoard(bvm.Id).Wait();
                });

            boardsChanges.Connect();

            ////////////////////
            // Columns AutoSaver
            ////////////////////
            var columnsChanges = Columns.Connect().Publish();
            SubscribeChanged(columnsChanges,
                cvm =>
                {
                    mon.LogicVerbose($"Box.Columns.ItemChanged {cvm.Id}::{cvm.Name}::{cvm.Order}");
                    var ci = mapper.Map<ColumnViewModel, Column>(cvm);
                    repo.CreateOrUpdateColumn(ci).Wait();
                });

            SubscribeAdded(columnsChanges,
                cvm =>
                {
                    mon.LogicVerbose($"Box.Columns.Add {cvm.Id}::{cvm.Name}::{cvm.Order}");

                    var ci = mapper.Map<ColumnViewModel, Column>(cvm);
                    ci = repo.CreateOrUpdateColumn(ci).Result;

                    cvm.Id = ci.Id;
                });

            SubscribeRemoved(columnsChanges,
                cvm =>
                {
                    mon.LogicVerbose($"Box.Columns.Remove {cvm.Id}::{cvm.Name}::{cvm.Order}");

                    repo.DeleteColumn(cvm.Id).Wait();
                });


            columnsChanges.Connect();

            /////////////////
            // Rows AutoSaver
            /////////////////
            var rowsChanges = Rows.Connect().Publish();
            SubscribeChanged(rowsChanges,
                rvm =>
                {
                    mon.LogicVerbose($"Box.Rows.ItemChanged {rvm.Id}::{rvm.Name}::{rvm.Order}");
                    var row = mapper.Map<RowViewModel, Row>(rvm);
                    repo.CreateOrUpdateRow(row).Wait();
                });

            SubscribeAdded(rowsChanges,
                rvm =>
                {
                    mon.LogicVerbose($"Box.Rows.Add {rvm.Id}::{rvm.Name}::{rvm.Order}");

                    var ri = mapper.Map<RowViewModel, Row>(rvm);
                    ri = repo.CreateOrUpdateRow(ri).Result;

                    rvm.Id = ri.Id;
                });

            SubscribeRemoved(rowsChanges,
                rvm =>
                {
                    mon.LogicVerbose($"Box.Rows.Remove {rvm.Id}::{rvm.Name}::{rvm.Order}");

                    repo.DeleteRow(rvm.Id).Wait();
                });

            rowsChanges.Connect();

            //////////////////
            // Cards AutoSaver
            //////////////////
            var cardsChanges = Cards.Connect().Publish();
            SubscribeChanged(cardsChanges,
                cvm =>
                {
                    mon.LogicVerbose($"Box.Cards.ItemChanged {cvm.Id}::{cvm.Header}");
                    var iss = mapper.Map<CardViewModel, Card>(cvm);
                    repo.CreateOrUpdateCard(iss).Wait();
                });

            SubscribeAdded(cardsChanges,
                cvm =>
                {
                    mon.LogicVerbose($"Box.Cards.Add {cvm.Id}::{cvm.Header}");
                    var ci = mapper.Map<CardViewModel, Card>(cvm);
                    ci = repo.CreateOrUpdateCard(ci).Result;

                    cvm.Id = ci.Id;
                });

            SubscribeRemoved(cardsChanges,
                cvm =>
                {
                    mon.LogicVerbose($"Box.Cards.Remove {cvm.Id}::{cvm.Header}");

                    repo.DeleteCard(cvm.Id).Wait();
                });

            cardsChanges.Connect();
            InitializeLogger(repo);

        }

        public async Task Load(ILoadRepository repo)
        {
            var box = await repo.Load();
            Load(box);
        }

        public void Load(Box box)
        {
            Cards.AddRange(box.Cards.Select(x => mapper.Map<Card, CardViewModel>(x)));
            Columns.AddRange(box.Columns.Select(x => mapper.Map<Column, ColumnViewModel>(x)));
            Rows.AddRange(box.Rows.Select(x => mapper.Map<Row, RowViewModel>(x)));
            Boards.AddRange(box.Boards.Select(x => mapper.Map<Board, BoardViewModel>(x)));
            LogEntries.AddRange(box.Log.Select(x => mapper.Map<Kamban.Repository.Models.LogEntry, LogEntryViewModel>(x)));

            Loaded = true;
        }

        private static IDisposable SubscribeChanged<T>(IObservable<IChangeSet<T>> x,
            Action<T> a)
            where T : INotifyPropertyChanged
        {
            return x.WhenAnyAutoSavePropertyChanged()
                .Subscribe(a);
        }

        private static IDisposable SubscribeAdded<T>(IObservable<IChangeSet<T>> x,
            Action<T> a)
            where T : INotifyPropertyChanged
        {
            return x.WhereReasonsAre(ListChangeReason.Add, ListChangeReason.AddRange)
                .Subscribe(c =>
                {
                    var vms = c.SelectMany(q =>
                    {
                        var list = new List<T>();
                        if (q.Range != null)
                            list.AddRange(q.Range);
                        if (q.Item.Current != null)
                            list.Add(q.Item.Current);
                        return list;
                    });

                    foreach (var cvm in vms)
                        a(cvm);
                });
        }

        private static IDisposable SubscribeRemoved<T>(IObservable<IChangeSet<T>> x,
            Action<T> a)
            where T : INotifyPropertyChanged
        {
            return x.WhereReasonsAre(ListChangeReason.Remove)
                .Subscribe(c => c
                    .Select(q => q.Item.Current)
                    .ToList()
                    .ForEach(a));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewRequest"></param>
        /// 
        private void Repo_CreateOrUpdateLogEntry(ISaveRepository repo, LogEntryViewModel Entry)
        {
            var en = mapper.Map<LogEntryViewModel, LogEntry>(Entry);
            en = repo.CreateOrUpdateLogEntry(en).Result;

            Entry.Id = en.Id;
        }

        public void InitializeLogger(ISaveRepository repo)
        {
            /////////////////////////////////////////
            /// Mirror LogEntries to repo
            /////////////////////////////////////////

            LogEntries
                 .Connect()
                 .WhereReasonsAre(ListChangeReason.Add | ListChangeReason.AddRange)
                 .Subscribe(ch =>
                 {
                     foreach (Change<LogEntryViewModel> c in ch)
                     {
                         LogEntryViewModel Entry = c.Item.Current;
                         if (Entry != null)
                         {
                             Repo_CreateOrUpdateLogEntry(repo, Entry);
                         }
                         if (c.Range != null)
                         {
                             foreach (LogEntryViewModel REntry in c.Range.ToList())
                             {
                                 Repo_CreateOrUpdateLogEntry(repo, REntry);
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
                            Id = LogEntries.Count + 1,   // Fixme not working if log entries are deleted
                            Time = ccvm.Item.Current.Created,
                            ColumnId = ccvm.Item.Current.ColumnDeterminant,
                            RowId = ccvm.Item.Current.RowDeterminant,
                            Source = ccvm.Reason.ToString(),
                            Note = ccvm.Item.Current.Header
                        };

                      /*  Entry.Column = Columns.Items.Where(x => { return (x.Id == Entry.ColumnId); }).First().Name;
                        Entry.Row = Rows.Items.Where(x => { return (x.Id == Entry.RowId); }).First().Name;
                        Entry.Board = Rows.Items.Where(x => { return (x.Id == Entry.BoardId); }).First().Name;
                        */

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
                    LogEntries.Add(
                        new LogEntryViewModel()
                        {
                            Time = ccvm.Item.Current.Created,
                            ColumnId = ccvm.Item.Current.ColumnDeterminant,
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
                                         new LogEntryViewModel()
                                         {
                                             Time = cvm.Created,
                                             ColumnId = cvm.ColumnDeterminant,
                                             RowId = cvm.RowDeterminant,
                                             Source = ccvm.Reason.ToString(),
                                             Note = cvm.Header
                                         });
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
                                Time = DateTime.Now,
                                Property = c.PropertyName,
                                OldValue = cvm.GetType().GetProperty(c.PropertyName).GetValue(cvm, null)?.ToString(),

                                ColumnId = cvm.ColumnDeterminant,
                                RowId = cvm.RowDeterminant,
                                BoardId = cvm.BoardId,
                                CardId = cvm.Id,
                            }); ; ;


            });


            cvm.Changed.Subscribe(c =>
            {
                if (notLogging.Contains(c.PropertyName)) return;

                var Entry = LogEntries.Items.Where(x => { return (x.Property == c.PropertyName) & (x.CardId == cvm.Id); }).Last();

                if (Entry == null) // Should not happen
                {
                    Entry = new LogEntryViewModel()
                    {
                        Time = DateTime.Now,
                        Property = c.PropertyName,
                        OldValue = "",

                        ColumnId = cvm.ColumnDeterminant,
                        RowId = cvm.RowDeterminant,
                        BoardId = cvm.BoardId,
                        CardId = cvm.Id,

                        Source = "Card Changed",
                    };
                };

                Entry.NewValue = cvm.GetType().GetProperty(c.PropertyName).GetValue(cvm, null)?.ToString();
                Entry.Note = "Property [" + c.PropertyName + "]: " + Entry.OldValue + " -> " + Entry.NewValue;

                Entry.Column = Columns.Items.Where(x => { return (x.Id == Entry.ColumnId); }).First().Name;
                Entry.Row = Rows.Items.Where(x => { return (x.Id == Entry.RowId); }).First().Name;
                Entry.Board = Rows.Items.Where(x => { return (x.Id == Entry.BoardId); }).First().Name;

            });
        }
    }
}
