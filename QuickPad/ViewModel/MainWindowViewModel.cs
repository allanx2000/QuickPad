using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuickPad.ViewModel
{
    public class MainWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private ObservableCollection<Document> tabs = new ObservableCollection<Document>();
        private Window window;

        public Document CurrentTab
        {
            get
            {
                return Get<Document>();
            }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public ICollection<Document> Tabs
        {
            get
            {
                return tabs;
            }
        }

        public MainWindowViewModel(Window window)
        {
            this.window = window;

            AddDocument();
            AddDocument();
        }

        private void AddDocument()
        {
            string title = "Document " + (Tabs.Count + 1);

            var doc = new Document(title);
            tabs.Add(doc);
            CurrentTab = doc;
        }

        public ICommand SaveAllCommand
        {
            get
            {
                return new CommandHelper(SaveAll);
            }
        }

        private void SaveAll()
        {
            foreach (var t in tabs)
            {
                t.Saved();
            }
        }
    }
}
