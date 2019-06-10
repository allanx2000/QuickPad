using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Windows.Input;

namespace QuickPad.ViewModel
{
    internal class RenameDocumentWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private Document doc;
        private RenameDocumentWindow window;

        public string Name
        {
            get => Get<string>();
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public RenameDocumentWindowViewModel(Document doc, RenameDocumentWindow window)
        {
            this.doc = doc;
            Name = doc.Name;
            this.window = window;
        }

        public ICommand ChangeCommand
        {
            get
            {
                return new CommandHelper(ChangeName);
            }
        }

        private void ChangeName()
        {
            if (string.IsNullOrEmpty(Name))
                MessageBoxFactory.ShowError("Name cannot be empty.");
            else
            {
                doc.SetName(Name);
                doc.HasChanges = true;
                window.Close();
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new CommandHelper(() => window.Close());
            }
        }
    }
}