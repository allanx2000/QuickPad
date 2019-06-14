using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Innouvous.Utils;
using QuickPad.ViewModel;

namespace QuickPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            vm = new MainWindowViewModel(this);
            DataContext = vm;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (vm.HasUnsaved)
            {
                var ctnu = MessageBoxFactory.ShowConfirmAsBool("There are unsaved documents. Continue closing?", "Unsaved Documents", MessageBoxImage.Exclamation);
                if (!ctnu)
                    e.Cancel = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.PageDown)
                {
                    vm.NextTabCommand.Execute(null);
                    (sender as TextBox).Focus();
                }
                else if (e.Key == Key.PageUp)
                {
                    vm.PreviousTabCommand.Execute(null);
                    (sender as TextBox).Focus();
                }
            }
        }
    }
}
