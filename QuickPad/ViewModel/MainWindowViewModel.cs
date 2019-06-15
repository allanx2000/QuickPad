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
        private bool tabDeleted = false;
        private const string New = "Untitled";

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

        public int ContentFontSize
        {
            get => settings.FontSize;
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
                
                using (StreamReader sr = new StreamReader(settings.SaveFile)) {
                    SavedDocuments docs = (SavedDocuments) jser.Deserialize(sr, typeof(SavedDocuments));

                    foreach (var d in docs.Documents)
                    {
                        d.HasChanges = false;
                        Tabs.Add(d);

                    }

                    if (Tabs.Count > 0)
                    {
                        CurrentTab = Tabs.First();
                    }
                }
            }
        }

        private void AddDocument()
        {
            var doc = new Document(New);
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
            try
            {
                if (string.IsNullOrEmpty(settings.SaveFile))
                {
                    MessageBoxFactory.ShowInfo(window, "Save Path is not set. Please set now.", "Save Path Not Set");

                    ShowSettings(false);

                    if (string.IsNullOrEmpty(settings.SaveFile))
                    {
                        Log("Save Path was not set.");
                        return;
                    }
                }

                if (!HasUnsaved && !tabDeleted)
                {
                    Log("No documents need to be saved");
                    return;
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

                Log("All documents saved");
                tabDeleted = false;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e, "Save Error", owner: window);
                Log("All documents saved");
            }
        }

        private void Log(string msg)
        {
            StatusText = $"{DateTime.Now.ToShortTimeString()}: {msg}"; 
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

        public ICommand AboutCommand
        {
            get => new CommandHelper(ShowAboutWindow);
        }

        private void ShowAboutWindow()
        {
            try
            {
                using (StreamReader sr = new StreamReader("About.md"))
                {
                    var dlg = new Innouvous.Utils.MarkdownViewer.Viewer("About", sr.ReadToEnd());
                    dlg.Owner = window;
                    dlg.ShowDialog();
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
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

        public bool HasUnsaved {
            get
            {
                return tabDeleted || Tabs.Where(x => x.HasChanges).Count() > 0;
            }
        }

        public void CloseDocument()
        {
            if (CurrentTab != null)
            {
                if (MessageBoxFactory.ShowConfirmAsBool($"Delete {CurrentTab.Name}? Change will not be saved until workspace is Saved", "Confirm Remove", MessageBoxImage.Exclamation))
                {
                    Tabs.Remove(CurrentTab);
                    tabDeleted = true;

                    if (Tabs.Count > 0)
                        CurrentTab = Tabs.ElementAt(Tabs.Count - 1);
                    else
                        CurrentTab = null;

                    //SaveAll();
                }
            }
        }

        public ICommand RenameDocumentCommand
        {
            get => new CommandHelper(RenameDocument);
        }

        private void RenameDocument()
        {
            if (CurrentTab != null)
            {
                var dlg = new RenameDocumentWindow(CurrentTab);
                dlg.Owner = window;
                dlg.ShowDialog();
            }
        }

        public ICommand NextTabCommand
        {
            get => new CommandHelper(GotoNextTab);
        }

        private void GotoNextTab()
        {
            if (CurrentTab != null && tabs.Count > 1)
            {
                bool found = false;
                foreach (var t in tabs)
                {
                    if (t == CurrentTab)
                    {
                        found = true;
                    }
                    else if (found)
                    {
                        CurrentTab = t;
                        break;
                    }   
                }
            }
        }

        public ICommand PreviousTabCommand
        {
            get => new CommandHelper(GotoPreviousTab);
        }

        private void GotoPreviousTab()
        {
            if (CurrentTab != null && tabs.Count > 1)
            {
                bool found = false;
                foreach (var t in tabs.Reverse())
                {
                    if (t == CurrentTab)
                    {
                        found = true;
                    }
                    else if (found)
                    {
                        CurrentTab = t;
                        break;
                    }
                }
            }
        }
    }

}
