using Kamban.ViewModels;
using System.Windows;




namespace Kamban.Views
{
    public partial class HeaderPropertyWindow : Window
    {
        public HeaderPropertyWindow()
        {
            InitializeComponent();
        }

        public HeaderPropertyWindow(HeaderPropertyViewModel vm)
        {
            InitializeComponent();
            this.headerPropertyView.setViewModel(vm);
        }


    }
}
