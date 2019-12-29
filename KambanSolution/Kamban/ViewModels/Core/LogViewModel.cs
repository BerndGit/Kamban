using DynamicData;
using DynamicData.Binding;
using Kamban.ViewRequests;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;
using Ui.Wpf.Common.ViewModels;


namespace Kamban.ViewModels.Core
{
    public partial class LogViewModel : ViewModelBase, IInitializableViewModel 
    {
        public const string LogViewId = "KambanLogView";

        public ReadOnlyObservableCollection<LogEntryViewModel> LogEntries { get; set; } // = new ReadOnlyObservableCollection<ILogEntry>();
        public ReadOnlyObservableCollection<LogEntryViewModel> FilteredLogEntries { get; set; }

        public BoxViewModel Box;

        public ReadOnlyObservableCollection<ColumnViewModel> AvailableColumns { get; set; }
        public ReadOnlyObservableCollection<RowViewModel> AvailableRows { get; set; }
        public ReadOnlyObservableCollection<CardViewModel> AvailableCards { get; set; }

        SourceList<ColumnViewModel> noColumn { get; set; } = new SourceList<ColumnViewModel>();
        SourceList<RowViewModel> noRow { get; set; } = new SourceList<RowViewModel>();
        SourceList<CardViewModel> noCard { get; set; } = new SourceList<CardViewModel>();


        [Reactive] public ColumnViewModel FltSelectedColumn { get; set; }
        [Reactive] public RowViewModel FltSelectedRow { get; set; }
        [Reactive] public CardViewModel FltSelectedCard { get; set; }
        [Reactive] public Nullable<bool> FltAutomatic { get; set; }

        [Reactive] public ColumnViewModel EdtSelectedColumn { get; set; }
        [Reactive] public RowViewModel EdtSelectedRow { get; set; }
        [Reactive] public CardViewModel EdtSelectedCard { get; set; }

      

        [Reactive] public String EdtTopic { get; set; }
            
        [Reactive] public String EdtNote { get; set; }
        [Reactive] public DateTime EdtTime { get; set; }

        [Reactive] public DateTime CurrTime { get; set; }

        private IObservable<DateTime> _CurrTime { get; set; } = Observable.Interval(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
           .Select(__ => DateTime.Now);

        [Reactive] public Boolean AddCurrentTIme { get; set; } = true;

        public bool EntryEditable {
            get
            {
                if (SelectedLogEntry == null) return false;
                return !SelectedLogEntry.Automatic;
            }
        }

        private LogEntryViewModel _SelectedLogEntry;
        public LogEntryViewModel SelectedLogEntry
        { 
            get => _SelectedLogEntry;
            set
            {
                if (_SelectedLogEntry == value) return;

                this.RaisePropertyChanging(nameof(SelectedLogEntry));
                _SelectedLogEntry = value;
                
                this.RaisePropertyChanged(nameof(SelectedLogEntry));
                this.RaisePropertyChanged(nameof(SelectedLogEntryColumn));
                this.RaisePropertyChanged(nameof(SelectedLogEntryRow));
                this.RaisePropertyChanged(nameof(SelectedLogEntryCard));
                this.RaisePropertyChanged(nameof(EntryEditable));
                this.RaisePropertyChanged(nameof(EdtTopic));
                this.RaisePropertyChanged(nameof(EdtTopic));
                this.RaisePropertyChanged(nameof(EdtNote));
                this.RaisePropertyChanged(nameof(EdtTime));
            }
        } 

        private ColumnViewModel _SelectedLogEntryColumn;
        public ColumnViewModel SelectedLogEntryColumn
        {
            get
            {
                try
                {
                    return Box.Columns.Items.Where(col => (col.Id == SelectedLogEntry.ColumnId)).First();
                }
                catch
                {
                    return null;
                }

            }
            set
            {
                if (SelectedLogEntry != null)
                {
                    if (value == null)
                    {
                        SelectedLogEntry.ColumnId = -1;
                        SelectedLogEntry.Column = "";
                    }
                    else
                    {
                        SelectedLogEntry.Column = value.Name;
                        SelectedLogEntry.ColumnId = value.Id;
                        
                    }
                }
                this.RaiseAndSetIfChanged(ref _SelectedLogEntryColumn, value);
                this.RaisePropertyChanged(nameof(SelectedLogEntry));
                
            }
        }

        private RowViewModel _SelectedLogEntryRow;
        public RowViewModel SelectedLogEntryRow
        {
            get
            {
                try
                {
                    return Box.Rows.Items.Where(col => (col.Id == SelectedLogEntry.RowId)).First();
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (SelectedLogEntry != null)
                {
                    if (value == null)
                    {
                        SelectedLogEntry.RowId = -1;
                        SelectedLogEntry.Row = "";
                    }
                    else
                    {
                        SelectedLogEntry.Row = value.Name;
                        SelectedLogEntry.RowId = value.Id;
                        
                    }
                }
                this.RaiseAndSetIfChanged(ref _SelectedLogEntryRow, value);
                this.RaisePropertyChanged(nameof(SelectedLogEntry));

            }
        }

        private CardViewModel _SelectedLogEntryCard;
        public CardViewModel SelectedLogEntryCard
        {
            get
            {
                try
                {
                    return Box.Cards.Items.Where(col => (col.Id == SelectedLogEntry.CardId)).First();
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (SelectedLogEntry != null)
                {
                    if (value == null)
                    {
                        SelectedLogEntry.ColumnId = -1;
                    }
                    else
                    {
                        SelectedLogEntry.CardId = value.Id;
                        
                    }
                }
                this.RaiseAndSetIfChanged(ref _SelectedLogEntryCard, value);
                this.RaisePropertyChanged(nameof(SelectedLogEntry));



            }
        }


        ///      public ReactiveCommand<Unit, Unit> UpdateFilteredList { get; private set; }

        public ReadOnlyObservableCollection<String> TopicProposals { get; set;  }


        private Func<LogEntryViewModel, bool> MakeRowFilter(RowViewModel row)
        {
            if (row == null) return logEntry => true;
            if (row.Id == -1) return logEntry => true;
            return logEntry => logEntry.RowId == row.Id;
        }

        private Func<LogEntryViewModel, bool> MakeColumnFilter(ColumnViewModel col)
        {
            if (col == null) return logEntry => true;
            if (col.Id == -1) return logEntry => true;
            return logEntry => logEntry.ColumnId == col.Id;
        }

        private Func<LogEntryViewModel, bool> MakeAutomaticFilter(Nullable<bool> auto)
        {
            if (auto == null) return logEntry => true;
            return logEntry => logEntry.Automatic == auto;
        }

        private Func<LogEntryViewModel, bool> MakeCardFilter(CardViewModel card)
        {
            if (card == null) return logEntry => true;
            if (card.Id == -1) return logEntry => true;
            return logEntry => logEntry.CardId == card.Id;
        }

        private Func<String, bool> TopicProposalFilter(String topic)
        {
           //  if (topic != null)  MessageBox.Show("topic = " + topic); 
           //  else MessageBox.Show("topic = " +"null");

            if (topic == null) return _topic => true;
            if (topic.Length == 0) return _topic => true;
            return _topic => (_topic !=null) ? _topic.ToUpper().Contains(topic.Trim().ToUpper()) : false;
            

        }
        

        private class EntryTimeComparer : IComparer<ILogEntry>
        {
            public int Direction  {get; set; }= 1;

            int IComparer<ILogEntry>.Compare(ILogEntry x, ILogEntry y)
            {
                return x.Time.CompareTo(y.Time)* Direction;
            }
        }

        public void Initialize(ViewRequest viewRequest)
        {
            Box = ((LogViewRequest)viewRequest).Box;
            Title = "Log: " + Box.Title;

            Box.LogEntries.Connect()
                .Bind(out ReadOnlyObservableCollection<LogEntryViewModel> temp0);

            this.LogEntries = temp0;

            /// Filter Results

            noColumn.Add(new ColumnViewModel() { Name = "-- none --", Order = -1, Id = -1 });
            noRow.Add(new RowViewModel() { Name = "-- none --", Order = -1, Id = -1 });
            noCard.Add(new CardViewModel() { Header = "-- none --", Order = -1, Id = -1 });

            //     UpdateFilteredList = ReactiveCommand.Create(delegate { UpdateFilteredListExecute(); });

            Box.Columns
             .Connect()
             .Or(noColumn.Connect())
             .Sort(SortExpressionComparer<ColumnViewModel>.Ascending(x => x.Order))
             .Bind(out ReadOnlyObservableCollection<ColumnViewModel> temp1)
             .Subscribe();

            AvailableColumns = temp1;

            Box.Rows
             .Connect()
             .Or(noRow.Connect())
             .Sort(SortExpressionComparer<RowViewModel>.Ascending(x => x.Order))
             .Bind(out ReadOnlyObservableCollection<RowViewModel> temp2)
             .Subscribe();

            AvailableRows = temp2;

            Box.Cards
             .Connect()
             .Or(noCard.Connect())
             .Sort(SortExpressionComparer<CardViewModel>.Ascending(x => x.Id))
             .Bind(out ReadOnlyObservableCollection<CardViewModel> temp3)
             .Subscribe();

            AvailableCards = temp3;


            var observableRowFilter = this.WhenAnyValue(viewModel => viewModel.FltSelectedRow)
                 .Select(MakeRowFilter);

            var observableColFilter = this.WhenAnyValue(viewModel => viewModel.FltSelectedColumn)
                 .Select(MakeColumnFilter);

            var observableAutomaticFilter = this.WhenAnyValue(viewModel => viewModel.FltAutomatic)
                 .Select(MakeAutomaticFilter);

            var observableCardFilter = this.WhenAnyValue(viewModel => viewModel.FltSelectedCard)
                .Select(MakeCardFilter);

            Box.LogEntries.Connect()
                .Filter(observableRowFilter)
                .Filter(observableColFilter)
                .Filter(observableAutomaticFilter)
                .Filter(observableCardFilter)
                .Sort(new EntryTimeComparer(){ Direction = -1})
                .Bind(out ReadOnlyObservableCollection<LogEntryViewModel> temp4)
                .Subscribe();
            FilteredLogEntries = temp4;

           IObservable<Func<string, bool>> observableTopicProposalFilter =
                        this.WhenAnyValue(x => x.SelectedLogEntry.Topic)
  //                      .Throttle(TimeSpan.FromMilliseconds(100))
                        .Select(TopicProposalFilter);

           observableTopicProposalFilter.Subscribe(/* x => { System.Windows.MessageBox.Show("observableTopicProposalFilter updated"); }*/  );
                  





                Box.LogEntries.Connect()
                       .Transform(x => x.Topic)
                       .Sort(SortExpressionComparer<String>.Ascending(t => t))
                       .Filter(observableTopicProposalFilter)
                       .Distinct(x => x)
                       .Bind(out ReadOnlyObservableCollection<String> temp5)
                       .Subscribe();

                   TopicProposals = temp5;


            //// Add/Edit entries ==================================================================

            _CurrTime.Where(_ => AddCurrentTIme).Subscribe(t => { EdtTime = t; });
        }

        public void MakeNewEntry()
        {
            LogEntryViewModel entr = new LogEntryViewModel()
            {
                Time = DateTime.Now,
                Automatic = false,
                Row = FltSelectedRow?.Name,
                RowId = FltSelectedRow?.Id ?? -1,
                Column = FltSelectedColumn?.Name,
                ColumnId = FltSelectedColumn?.Id ?? -1,
                CardId = FltSelectedCard?.Id ?? -1
            };

            Box.LogEntries.Add(entr);
            SelectedLogEntry = entr;
        }


    }





}
