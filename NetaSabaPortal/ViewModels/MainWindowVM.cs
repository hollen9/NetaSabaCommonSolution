using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using NetaSabaPortal.Options;
using SteamKit2.Internal;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace NetaSabaPortal.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        public IOptions<PathOptions> _dirOptions;
        private string _steamPath;
        private bool _isSteamPathValid;
        private string _cs2Path;
        private bool _isCS2PathValid;
        private string _cs2acfPath;
        private bool _isCS2AcfPathValid;

        public MainWindowVM(IOptions<PathOptions> dirOptions)
        {
            _dirOptions = dirOptions;
            
            CS2AcfPath = _dirOptions.Value.Cs2acf;
            CS2Path = _dirOptions.Value.Cs2;
            SteamPath = _dirOptions.Value.Steam;
        }
        public string SteamPath { get => _steamPath; set => SetProperty(ref _steamPath, value); }
        public bool IsSteamPathValid { get => _isSteamPathValid; set => SetProperty(ref _isSteamPathValid, value); }
        public string CS2Path { get => _cs2Path; set => SetProperty(ref _cs2Path, value); }
        public bool IsCS2PathValid { get => _isCS2PathValid; set => SetProperty(ref _isCS2PathValid, value); }
        public string CS2AcfPath { get => _cs2acfPath; set => SetProperty(ref _cs2acfPath, value); }
        public bool IsCS2AcfPathValid { get => _isCS2AcfPathValid; set => SetProperty(ref _isCS2AcfPathValid, value); }
        

        public ICommand BrowseCommand => new RelayCommand<string>((typesConcat) =>
        {
            if (string.IsNullOrEmpty(typesConcat))
            {
                return;
            }
            var types = typesConcat.Split(',');
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (types.Contains("steam"))
            {
                // Let user select steam.exe
                dialog.Filter = "Steam Client|steam.exe";
                dialog.Title = "Select Steam Client";
                dialog.InitialDirectory = @"C:\";
                if (dialog.ShowDialog() != true)
                {
                    return;
                }
                SteamPath = dialog.FileName;
            }
            if (types.Contains("cs2"))
            {
                // Let user select cs2.exe or csgo.exe at the same time
                dialog.Filter = "CS2/CSGO Client|cs2.exe; csgo.exe";
                dialog.Title = "Select CS2/CSGO Client";
                string steamDir = IsSteamPathValid ? Path.GetDirectoryName(SteamPath) : null;
                string steamGameDir = Path.Combine(steamDir, @"steamapps\common\");

                dialog.InitialDirectory = IsSteamPathValid ? steamGameDir : @"C:\";
                if (dialog.ShowDialog() != true)
                {
                    return;
                }
                CS2Path = dialog.FileName;
            }
            if (types.Contains("cs2acf"))
            {
                // Let user select appworkshop_730.acf
                dialog.Filter = "CS2 Workshop ACF|appworkshop_730.acf";
                dialog.Title = "Select appworkshop_730.acf";
                string cs2Dir = IsCS2PathValid ? Path.GetDirectoryName(CS2Path) : null;
                string cs2WorkshopDir = Path.Combine(cs2Dir, @"\..\..\workshop");

                dialog.InitialDirectory = IsCS2PathValid ? cs2WorkshopDir : @"C:\";
                if (dialog.ShowDialog() != true)
                {
                    return;
                }
                CS2AcfPath = dialog.FileName;
            }
            
            
        });
        public ICommand AutoCommand => new RelayCommand<string>(AutoSetupPath);

        private void AutoSetupPath(string? typesConcat)
        {
            if (string.IsNullOrEmpty(typesConcat))
            {
                return;
            }
            var types = typesConcat.Split(',');
            if (types.Contains("steam") && !IsSteamPathValid)
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
                    // MessageBox.Show("Steam Client Installation Folder not found. Please select manually.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (types.Contains("cs2") && !IsCS2PathValid)
            {
                if (!string.IsNullOrEmpty(CS2Path) && Path.GetFileName(CS2Path).ToLower() == "csgo.exe")
                {
                    string possibleCs2path = Path.Combine(Path.GetDirectoryName(CS2Path), @"game\bin\win64\cs2.exe");
                    CS2Path = possibleCs2path;
                }
                else
                {
                    if (IsSteamPathValid)
                    {
                        string steamDir = Path.GetDirectoryName(SteamPath);
                        string steamGameDir = Path.Combine(steamDir, @"\steamapps\common\");
                        string cs2Path = steamGameDir + "Counter-Strike 2D\\cs2.exe";
                        if (System.IO.File.Exists(cs2Path))
                        {
                            CS2Path = cs2Path;
                        }
                        else
                        {
                            // Show Error Message on MsgBox
                            // MessageBox.Show("CS2 Client Installation Folder not found. Please select manually.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Show Error Message on MsgBox
                        // MessageBox.Show("Steam Client Installation Folder not found. Please select manually.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            if (types.Contains("cs2acf") && !IsCS2AcfPathValid)
            {
                if (IsCS2PathValid)
                {
                    string possibleAcfPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(CS2Path), @"..\..\..\..\..\workshop\appworkshop_730.acf"));
                    if (System.IO.File.Exists(possibleAcfPath))
                    {
                        CS2AcfPath = possibleAcfPath;
                    }
                    else
                    {
                        if (IsSteamPathValid)
                        {
                            string relatedToSteamPath = @"steamapps\workshop\appworkshop_730.acf";
                            string steamDir = Path.GetDirectoryName(SteamPath);
                            possibleAcfPath = Path.GetFullPath(Path.Combine(steamDir, relatedToSteamPath));
                            if (File.Exists(possibleAcfPath))
                            {
                                CS2AcfPath = possibleAcfPath;
                            }
                            else
                            {
                                // Show Error Message on MsgBox
                                // MessageBox.Show("CS2 Workshop ACF not found. Please select manually.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        // Show Error Message on MsgBox
                        // MessageBox.Show("CS2 Workshop ACF not found. Please select manually.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                // Get Steam Client Installation Folder
            }
            // Get Steam Client Installation Folder

        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SteamPath")
            {
                IsSteamPathValid = File.Exists(SteamPath) && Path.GetFileName(SteamPath).ToLower() == "steam.exe";
                if (IsSteamPathValid)
                {
                    AutoSetupPath("cs2");
                }
            }
            else if (e.PropertyName == "CS2Path")
            {
                bool isFileExists = File.Exists(CS2Path);
                bool isCs2 = Path.GetFileName(CS2Path).ToLower() == "cs2.exe";
                bool isCsgo = Path.GetFileName(CS2Path).ToLower() == "csgo.exe";

                IsCS2PathValid = isFileExists && isCs2;
                if (IsCS2PathValid)
                {
                    AutoSetupPath("cs2acf");
                }
                else if (isFileExists && isCsgo)
                {
                    AutoSetupPath("cs2");
                }
            }
            else if (e.PropertyName == "CS2AcfPath")
            {
                IsCS2AcfPathValid = File.Exists(CS2AcfPath);
            }
            base.OnPropertyChanged(e);
        }

    }
}
