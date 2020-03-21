using Kamban.ViewModels;
using Kamban.ViewModels.Core;
using ReactiveUI;
using System;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;
using Ui.Wpf.Common.ViewModels;

namespace Kamban.Views
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class HeaderPropertyView : UserControl, IView, IStretchedSizeView
    {
        public HeaderPropertyView()
        {
            InitializeComponent();
        }

        public HeaderPropertyView(HeaderPropertyViewModel viewModel) :this()
        {           
            setViewModel(viewModel);
        }

       

        public void setViewModel(HeaderPropertyViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        double IStretchedSizeView.StretchedWidth
        {
            get { return 400; }
        }

        double IStretchedSizeView.StretchedHeight
        {
            get { return 400; }
        }
        

        public IViewModel ViewModel { get; set; }

        UiShowOptions viewOptions;

        void IView.Configure(UiShowOptions options)
        {
            viewOptions = options;
            // throw new System.NotImplementedException();
        }



        private void HeaderSaveCommand_Click(object sender, RoutedEventArgs e)
        {        
            ((HeaderPropertyViewModel)ViewModel).HeaderSaveCommand.Execute().Subscribe();            
            Window.GetWindow(this)?.Close(); // Closing Window
        }

        private void HeaderCancelCommand_Click(object sender, RoutedEventArgs e)
        {
            ((HeaderPropertyViewModel)ViewModel).HeaderCancelCommand.Execute().Subscribe();
            Window.GetWindow(this)?.Close(); // Closing Window          
        }



        private void update_Bindings()
        {
            TB_HeaderName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
}
}
