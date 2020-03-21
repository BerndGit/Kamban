
using Kamban.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;
using System.Windows;
using Ui.Wpf.Common.ViewModels;

namespace Kamban.Views
{
    public partial class CardEditView
    {
        public CardEditView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel { get; set; }

        public CardEditView(CardEditViewModel viewModel) :this()
        {            
            setViewModel(viewModel);
        }


        public void setViewModel(CardEditViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;    
        }

        private void ButtonSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CardEditViewModel)ViewModel).SaveCommand.Execute().Subscribe();
            Window.GetWindow(this)?.Close(); // Closing Window   
        }

        private void ButtonCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CardEditViewModel)ViewModel).CancelCommand.Execute().Subscribe();
            Window.GetWindow(this)?.Close(); // Closing Window   
        }
    }
}
