using Kamban.ViewModels;
using System.Windows;




namespace Kamban.Views
{
    public partial class CardEditWindow : Window
    {
        public CardEditWindow()
        {
            InitializeComponent();
        }

        public CardEditWindow(CardEditViewModel vm)
        {
            InitializeComponent();
            this.cardEditView.setViewModel(vm);
        }


    }
}
