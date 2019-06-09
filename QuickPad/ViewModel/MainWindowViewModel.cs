using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using Newtonsoft.Json;
using QuickPad.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuickPad.ViewModel
{
    public class MainWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly ObservableCollection<Document> tabs = new ObservableCollection<Document>();
        private readonly Window window;

        private readonly Settings settings;
        private readonly JsonSerializer jser = new JsonSerializer();

        private int docCounter = 1;

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

        public string StatusText
        {
            get => Get<string>();
            private set
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

            this.settings = Settings.Default;

            LoadSavedDocuments();
        }

        private void LoadSavedDocuments()
        {
            if (!string.IsNullOrEmpty(settings.SaveFile) && File.Exists(settings.SaveFile))
            {
                Tabs.Clear();
                docCounter = 1;

                using (StreamReader sr = new StreamReader(settings.SaveFile)) {
                    SavedDocuments docs = (SavedDocuments) jser.Deserialize(sr, typeof(SavedDocuments));

                    foreach (var d in docs.Documents)
                    {
                        d.HasChanges = false;
                        d.SetName(GetNewName());
                        Tabs.Add(d);

                    }

                    if (Tabs.Count > 0)
                    {
                        CurrentTab = Tabs.First();
                        docCounter = Tabs.Count + 1;
                    }
                }
            }
        }

        private void AddDocument()
        {
            string title = GetNewName();

            var doc = new Document(title);
            tabs.Add(doc);
            CurrentTab = doc;
        }

        private string GetNewName()
        {
            return "Doc " + docCounter++;
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
            if (string.IsNullOrEmpty(settings.SaveFile))
            {
                MessageBoxFactory.ShowInfo(window, "Save Path is not set. Please set now.", "Save Path Not Set");

                ShowSettings(false);

                if (string.IsNullOrEmpty(settings.SaveFile))
                {
                    StatusText = "Save Path was not set.";
                    return;
                }
            }

            //Save
            SavedDocuments saved = new SavedDocuments();
            saved.Documents.AddRange(tabs);

            var bak = settings.SaveFile + ".bak";
            foreach (var t in tabs) //Needed to remove the * from Name
            {
                t.HasChanges = false;
            }

            if (File.Exists(settings.SaveFile))
            {
                File.Copy(settings.SaveFile, bak, true);
            }

            using (StreamWriter sw = new StreamWriter(settings.SaveFile, false))
            {
                jser.Serialize(sw, saved);
            }

            

            StatusText = "All documents saved";
        }

        public ICommand SettingsCommand
        {
            get => new CommandHelper(() => ShowSettings(true));
        }

        private void ShowSettings(bool reloadNeeded)
        {
            var dlg = new SettingsWindow();
            dlg.Owner = window;
            dlg.ShowDialog();

            if (!dlg.Cancelled && reloadNeeded)
            {
                MessageBoxFactory.ShowInfo(window, "You need to restart the for changes to take effect.", "Restart Required");
            }

        }

        public ICommand AddDocumentCommand
        {
            get { return new CommandHelper(AddDocument); }
        }

        public ICommand CloseDocumentCommand
        {
            get { return new CommandHelper(CloseDocument); }
        }

        public void CloseDocument()
        {
            if (CurrentTab != null)
            {
                if (MessageBoxFactory.ShowConfirmAsBool($"Delete {CurrentTab.Name}? Change will not be saved until workspace is Saved", "Confirm Remove", MessageBoxImage.Exclamation))
                {
                    Tabs.Remove(CurrentTab);

                    if (Tabs.Count > 0)
                        CurrentTab = Tabs.ElementAt(Tabs.Count - 1);
                    else
                        CurrentTab = null;

                    //SaveAll();
                }
            }
        }
    }

}
