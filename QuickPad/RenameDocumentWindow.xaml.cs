using QuickPad.ViewModel;
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
using System.Windows.Shapes;

namespace QuickPad
{
    /// <summary>
    /// Interaction logic for RenameDocumentWindow.xaml
    /// </summary>
    public partial class RenameDocumentWindow : Window
    {
        private readonly RenameDocumentWindowViewModel vm;

        public RenameDocumentWindow(Document doc)
        {
            InitializeComponent();
            this.vm = new RenameDocumentWindowViewModel(doc, this);
            DataContext = vm;

            NameText.Focus();
        }
    }
}
