using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using NetaSabaPortal.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace NetaSabaPortal.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        public IOptions<DirectoryOptions> _dirOptions;
        private string _steamPath;
        private bool _isSteamPathValid;

        public MainWindowVM(IOptions<DirectoryOptions> dirOptions)
        {
            _dirOptions = dirOptions;
            SteamPath = _dirOptions.Value.Steam;
        }
        public string SteamPath { get => _steamPath; set => SetProperty(ref _steamPath, value); }
        public bool IsSteamPathValid { get => _isSteamPathValid; set => SetProperty(ref _isSteamPathValid, value); }


        public ICommand BrowseCommand => new RelayCommand<string>((typesConcat) =>
        {
            if (string.IsNullOrEmpty(typesConcat))
            {
                return;
            }
            var types = typesConcat.Split(',');
            if (types.Contains("steam"))
            {
                // Let user select steam.exe
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "Steam Client|steam.exe";
                dialog.Title = "Select Steam Client Installation Folder";
                dialog.InitialDirectory = @"C:\";
                if (dialog.ShowDialog() == true)
                {
                    SteamPath = dialog.FileName;
                }
            }
            if (types.Contains("cs2"))
            {
                // Get CS2 Installation Folder
            }
            if (types.Contains("cs2workshop"))
            {
                // Get CS2 Workshop Folder
            }
            else
            {
                // Get Steam Client Installation Folder
            }
            // Get Steam Client Installation Folder

        });
        public ICommand AutoCommand => new RelayCommand<string>((typesConcat) => 
        {
            if (string.IsNullOrEmpty(typesConcat))
            {
                return;
            }
            var types = typesConcat.Split(',');
            if (types.Contains("steam"))
            {
                // Find Steam Client Installation Folder automatically
                // Try with C: first
                // If not found, try with D:
                // If not found, try with E:...etc
                // If not found, ask user to select manually

                // do code

                string steamPath = string.Empty;
                
                // string csgoPath = disk + ":\\Program Files (x86)\\Steam\\steamapps\\common\\Counter-Strike Global Offensive\\csgo.exe";
                string[] possibleSteamFolders = new string[] 
                {
                    @":\Program Files (x86)\Steam\steam.exe",
                    @":\Program Files\Steam\steam.exe",
                    @":\Steam\steam.exe"
                };
                char diskStartingLetter = 'A';
                for (int i = 2; i < 26; i++)
                {
                    char disk = (char)((int)diskStartingLetter + i);
                    foreach (var possibleSteamFolder in possibleSteamFolders)
                    {
                        string possibleSteamPath = disk + possibleSteamFolder;
                        if (System.IO.File.Exists(possibleSteamPath))
                        {
                            steamPath = possibleSteamPath;
                            break;
                        }
                    }
                    if (steamPath != string.Empty)
                    {
                        SteamPath = steamPath;
                        break;
                    }
                    disk++;
                }
                if (steamPath == string.Empty)
                {
                    // Show Error Message on MsgBox
                    MessageBox.Show("Steam Client Installation Folder not found. Please select manually.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (types.Contains("cs2"))
            {
                // Get CS2 Installation Folder
            }
            if (types.Contains("cs2workshop"))
            {
                // Get CS2 Workshop Folder
            }
            else
            {
                // Get Steam Client Installation Folder
            }
            // Get Steam Client Installation Folder

        });

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SteamPath")
            {
                IsSteamPathValid = File.Exists(SteamPath);
            }

            base.OnPropertyChanged(e);
        }

    }
}
