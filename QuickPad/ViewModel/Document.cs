namespace QuickPad.ViewModel
{
    public class Document : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        public bool HasChanges
        {
            get => Get<bool>();
            private set
            {
                Set(value);
                RaisePropertyChanged("Name");
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
        }

        public void Saved()
        {
            HasChanges = false;
        }
    }
}