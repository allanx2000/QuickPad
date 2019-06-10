using Innouvous.Utils.MVVM;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Input;

namespace QuickPad.ViewModel
{
    public class Document : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        [JsonIgnore]
        private string description;

        [JsonIgnore]
        public string Description
        {
            get => description;
            private set
            {
                description = value;
                RaisePropertyChanged();
            }
        }

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
                UpdateDescription();
            }
        }

        public DateTime? Modified
        {
            get { return Get<DateTime?>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
                UpdateDescription();
            }
        }

        private void UpdateDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Created: " + Created.ToShortDateString());

            if (Modified.HasValue)
            {
                string modified = Modified.Value > DateTime.Today ? Modified.Value.ToShortTimeString() : Modified.Value.ToShortDateString();
                sb.AppendLine("Modified: " + modified);
            }
            Description = sb.ToString().Trim();
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