using DynamicData;
using DynamicData.Binding;
using Kamban.ViewRequests;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
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

        public ObservableCollection<ILogEntry> LogEntries { get; set; } = new ObservableCollection<ILogEntry>();
        public ReadOnlyObservableCollection<LogEntryViewModel> FilteredLogEntries { get; set; }

        public BoxViewModel Box;

        public ReadOnlyObservableCollection<ColumnViewModel> AvailableColumns { get; set; }
        public ReadOnlyObservableCollection<RowViewModel> AvailableRows { get; set; }
        public ReadOnlyObservableCollection<CardViewModel> AvailableCards { get; set; }

        SourceList<ColumnViewModel> noColumn { get; set; } = new SourceList<ColumnViewModel>();
        SourceList<RowViewModel> noRow { get; set; } = new SourceList<RowViewModel>();
        SourceList<CardViewModel> noCard { get; set; } = new SourceList<CardViewModel>();


        [Reactive] public ColumnViewModel SelectedColumn { get; set; }
        [Reactive] public RowViewModel SelectedRow { get; set; }
        [Reactive] public CardViewModel SelectedCard { get; set; }

        [Reactive] public Nullable<bool> FilerAutomatic { get; set; }





        

        public ReactiveCommand<Unit, Unit> UpdateFilteredList { get; private set; }

        private void UpdateFilteredListExecute()
        {
            System.Windows.MessageBox.Show("SelectedRowChangedExecute");

        }

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


        public void Initialize(ViewRequest viewRequest)
        {
            Box = ((LogViewRequest)viewRequest).Box;

            this.LogEntries.Clear();
            Box.LogEntries.Items.ToList().ForEach(ent => this.LogEntries.Add(ent));

            // Fixme
            Box.LogEntries.Connect().Subscribe(cl =>   
            {
                this.LogEntries.Clear();
                Box.LogEntries.Items.ToList().ForEach(ent => this.LogEntries.Add(ent));
            });

            Title = "Log: " + Box.Title;

            noColumn.Add(new ColumnViewModel() { Name = "-- none --" , Order=-1, Id = -1});
            noRow.Add(new RowViewModel() { Name = "-- none --", Order = -1, Id = -1 });
            noCard.Add(new CardViewModel() { Header = "-- none --", Order = -1, Id = -1 });

            UpdateFilteredList = ReactiveCommand.Create(delegate { UpdateFilteredListExecute(); });


            Box.Columns
             .Connect()
             .Or(noColumn.Connect())
             .AutoRefresh()
             .Sort(SortExpressionComparer<ColumnViewModel>.Ascending(x => x.Order))
             .Bind(out ReadOnlyObservableCollection<ColumnViewModel> temp1)
             .Subscribe();

            AvailableColumns = temp1;

            Box.Rows
             .Connect()
             .AutoRefresh()
             .Or(noRow.Connect())
             .Sort(SortExpressionComparer<RowViewModel>.Ascending(x => x.Order))
             .Bind(out ReadOnlyObservableCollection<RowViewModel> temp2)
             .Subscribe();

            AvailableRows = temp2;

            Box.Cards
             .Connect()
             .AutoRefresh()
             .Or(noCard.Connect())
             .Sort(SortExpressionComparer<CardViewModel>.Ascending(x => x.Order))
             .Bind(out ReadOnlyObservableCollection<CardViewModel> temp3)
             .Subscribe();

            AvailableCards = temp3;


            var observableRowFilter = this.WhenAnyValue(viewModel => viewModel.SelectedRow)
                 .Select(MakeRowFilter);

            var observableColFilter = this.WhenAnyValue(viewModel => viewModel.SelectedColumn)
                 .Select(MakeColumnFilter);

            var observableAutomaticFilter = this.WhenAnyValue(viewModel => viewModel.FilerAutomatic)
                 .Select(MakeAutomaticFilter);

            var observableCardFilter = this.WhenAnyValue(viewModel => viewModel.SelectedCard)
                .Select(MakeCardFilter);

            Box.LogEntries.Connect()
                .Filter(observableRowFilter)
                .Filter(observableColFilter)
                .Filter(observableAutomaticFilter)
                .Filter(observableCardFilter)
                .Bind(out ReadOnlyObservableCollection<LogEntryViewModel> temp4)
                .Subscribe();

            FilteredLogEntries = temp4;



        }



  /*      private void BoxLogChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            this.LogEntries.Clear();
            Box.LogEntries.Items.ToList().ForEach(ent => this.LogEntries.Add(ent));
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
        }*/
    }

 

  
    
}
