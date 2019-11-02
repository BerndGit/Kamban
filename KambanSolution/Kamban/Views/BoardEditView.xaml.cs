using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kamban.MatrixControl;
using Kamban.ViewModels;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;
using Ui.Wpf.Common.ViewModels;

namespace Kamban.Views
{
    public partial class BoardView : IView, IStretchedSizeView
    {
        public BoardView(BoardEditViewModel localBoardViewModel)
        {
            InitializeComponent();
            ViewModel = localBoardViewModel;
            DataContext = ViewModel;
        }

        public IViewModel ViewModel { get; set; }

        public void Configure(UiShowOptions options)
        {
            ViewModel.FullTitle = options.Title;
        }

        public double StretchedWidth
        {
            get
            {
                var starWidthTotal = Matrix.MainGrid.ColumnDefinitions.Skip(1)
                    .Select(x => x.Width.Value)
                    .Sum();
                var maxWidth = Matrix.MainGrid.ColumnDefinitions.Skip(1)
                    .Max(x => Math.Ceiling(x.ActualWidth * starWidthTotal / x.Width.Value));
                maxWidth += Matrix.MainGrid.ColumnDefinitions[0].ActualWidth;
                return maxWidth;
            }
        }

        public double StretchedHeight
        {
            get
            {
                var starHeightTotal = Matrix.MainGrid.RowDefinitions.Skip(1)
                    .Select(x => x.Height.Value)
                    .Sum();
                var maxHeight = Matrix.MainGrid.RowDefinitions.Skip(1)
                    .Max(rd => Math.Ceiling(rd.ActualHeight * starHeightTotal / rd.Height.Value));
                maxHeight += Matrix.MainGrid.RowDefinitions[0].ActualHeight;
                return maxHeight;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
