﻿using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuickPad.ViewModel
{
    public class SettingsWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Window window;
        private readonly Properties.Settings settings = Properties.Settings.Default;

        public bool Cancelled { get; private set; }

        public string SavePath
        {
            get => Get<string>();
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public string ExportPath
        {
            get => Get<string>();
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public int FontSize
        {
            get => Get<int>();
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public string HotKeyValue { get; set; }

        public SettingsWindowViewModel(Window window)
        {
            this.window = window;

            SavePath = settings.SaveFile;
            FontSize = settings.FontSize;
            HotKeyValue = settings.HotKey;
            Cancelled = true;
        }

        public ICommand CancelCommand
        {
            get => new CommandHelper(() => window.Close());
        }

        public ICommand SaveCommand
        {
            get => new CommandHelper(SaveSettings);
        }

        private void SaveSettings()
        {
            try
            {
                if (string.IsNullOrEmpty(SavePath) || !Path.IsPathRooted(SavePath) || IsInvalid(SavePath)){
                    throw new Exception("Path is not valid.");
                }
                else if (settings.SaveFile != SavePath)
                {
                    MessageBoxFactory.ShowInfo("This change will not take effect until the app is restarted.", "Save Path Changed");
                    settings.SaveFile = SavePath;
                    Cancelled = false;
                }

                if (FontSize != settings.FontSize)
                {
                    settings.FontSize = FontSize;
                    Cancelled = false;
                }

                if (!string.IsNullOrEmpty(HotKeyValue) && HotKeyValue != settings.HotKey)
                {
                    Enum.Parse(typeof(Key), HotKeyValue);
                    settings.HotKey = HotKeyValue;
                    Cancelled = false;
                }

                if (!Cancelled)
                    settings.Save();

                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        private bool IsInvalid(string savePath)
        {
            foreach (var c in Path.GetInvalidPathChars())
            {
                if (savePath.Contains(c))
                    return true;
            }

            return false;
        }

        public ICommand BrowseSaveFileCommand
        {
            get => new CommandHelper(BrowseSaveFile);
        }

        private void BrowseSaveFile()
        {
            var dlg = DialogsUtility.CreateSaveFileDialog(overwritePrompt: false, title: "Create/Select Save File");
            DialogsUtility.AddExtension(dlg, "QuickPad Save", "*.qps");

            var ok = dlg.ShowDialog(window);

            if (ok.HasValue && ok == true)
            {
                SavePath = dlg.FileName;
            }
        }
    }
}
