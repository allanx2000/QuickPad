using Innouvous.Utils.MVVM;
using Newtonsoft.Json;
using System;
using System.Windows.Input;

namespace QuickPad.ViewModel
{
    public class Document : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        public bool HasChanges
        {
            get => Get<bool>();
            set
            {
                Set(value);
                RaisePropertyChanged("Name");
            }
        }

        public DateTime Created
        {
            get { return Get<DateTime>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public DateTime Modified
        {
            get { return Get<DateTime>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        private string content;
        public string Content {
            get => content;
            set
            {
                //TODO: Compare Last Saved Content
                content = value;
                RaisePropertyChanged();
                HasChanges = true;
                Modified = DateTime.Now;
            }
        }

        public string Name
        {
            get
            {
                return Get<string>() + (HasChanges ? "*" : "");
            }
            private set
            {
                Set(value);
                RaisePropertyChanged("Header");
            }
        }

        public Document(string name, string content = null)
        {
            Name = name;

            HasChanges = false;

            this.content = content;
            Created = DateTime.Now;
        }

        internal void SetName(string v)
        {
            Name = v;
        }
    }
}