﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Nogic.WritableOptions;

using NetaSabaPortal.Options;

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
using System.Windows.Controls;
using System.Windows.Markup;
using static NetaSabaPortal.Options.EntitiesOptions;
using System.Collections.ObjectModel;
using NetaSabaPortal.Models;
using MaterialDesignThemes.Wpf;
using WPFLocalizeExtension.Engine;
using System.Windows.Threading;
using System.Net;
using QueryMaster.MasterServer;
using NetaSabaPortal.Helpers;
using System.Text.RegularExpressions;
using XamlAnimatedGif;
using NetaSabaPortal.Repositories;
using NetaSabaPortal.Models.Entities;
using Microsoft.Toolkit.Uwp.Notifications;
using Discord.Webhook;
using Discord;
using System.Globalization;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Windows.Media;
using ValveKeyValue;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.CompilerServices;
using NetaSabaPortal.Extensions;
using NetaSabaPortal.Resources.Locales;

namespace NetaSabaPortal.ViewModels
{
    public partial class MainWindowVM : ObservableObject
    {
        public IWritableOptions<PathOptions> _dirOptions;
        public IWritableOptions<EntitiesOptions> _entOptions;
        public IWritableOptions<WatcherOptions> _watcherOptions;
        public IWritableOptions<UiOptions> _uiOptions;
        public EditWatcherItemDialogVM _editWatcherItemDialogVM;
        //public IWritableOptions<DataOptions> _dataOptions;
        public Repositories.WatcherRepository _watcherRepository;
        public IOptions<AdvancedOptions> _advOptions;

        public MainWindowVM(
            IWritableOptions<PathOptions> dirOptions,
            IWritableOptions<EntitiesOptions> entOptions,
            IWritableOptions<WatcherOptions> watcherOptions,
            IWritableOptions<UiOptions> uiOptions,
            //IWritableOptions<DataOptions> dataOptions,
            EditWatcherItemDialogVM editWatcherItemDialogVM,
            Repositories.WatcherRepository watcherRepository,
            IOptions<AdvancedOptions> advOptions
            )
        {
            // This has to be called before any path variable changed event.
            _barMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));

            ExtractingProgressMax = 100;
            ExtractingProgressMin = 0;
            ExtractingProgressNow = 0;

            _dirOptions = dirOptions;
            _entOptions = entOptions;
            _watcherOptions = watcherOptions;
            _uiOptions = uiOptions;
            //_dataOptions = dataOptions;
            _watcherRepository = watcherRepository;
            _advOptions = advOptions;

            // Orders matter, as each path variable changed event will trigger AutoSetupPath
            CS2AcfPath = _dirOptions.Value.Cs2acf;
            CS2Path = _dirOptions.Value.Cs2;
            SteamPath = _dirOptions.Value.Steam;

            // Load Entities from config (option)
            EntitiesDefinitions = new ObservableCollection<EntityDefinition>(_entOptions.Value.Definitions);
            WatcherItems = new ObservableCollection<WatcherItem>(_watcherOptions.Value.List);


            _dispatcherTimer =
                new DispatcherTimer(TimeSpan.FromSeconds(_watcherOptions.Value.Interval),
                DispatcherPriority.Background, HandleWatcherCheck, App.Current.Dispatcher);
            WatcherInterval = _watcherOptions.Value.Interval;
            IsWatcherEnabled = _watcherOptions.Value.IsEnabled;
            WatcherNotifyCooldown = _watcherOptions.Value.NotifyCooldown;
            WatcherAutoJoinCooldown = _watcherOptions.Value.AutoJoinCooldown;
            IsTurnOffWatcherTimerAfterJoin = _watcherOptions.Value.IsTurnOffTimerAfterJoin;
            //_serverResponses = new Dictionary<string, OpenGSQ.Protocols.Source.IResponse>();

            _editWatcherItemDialogVM = editWatcherItemDialogVM;
            _editWatcherItemDialogVM.ShownChanged += (s) =>
            {
                IsWatcherEditDialogShown = _editWatcherItemDialogVM.IsShown;
            };
            _editWatcherItemDialogVM.ItemSaved += (s, e) =>
            {
                if (e.IsModifyMode)
                {
                    WatcherItems = new ObservableCollection<WatcherItem>(_watcherOptions.Value.List);
                }
                else
                {
                    WatcherItems.Add(e.WatcherItem);
                }
            };

            LocalizeDictionary.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Culture")
                {
                    HandleUiLanguageChanged(LocalizeDictionary.Instance.Culture);
                }
            };

            SelectedCulture = System.Globalization.CultureInfo.GetCultureInfo(uiOptions.Value.Language);
            LocalizeDictionary.Instance.Culture = SelectedCulture;
            Task.Run(async () => { await UpdateEntitiesListAsync(showInfoMessage: false, showErrorMessage: true); });
        }

        public string AppVersion => App.Current.Version.ToString();

        private void HandleUiLanguageChanged(CultureInfo culture)
        {
            OnPropertyChanged(nameof(SelectedEntity));
            // In order to refresh the UI, we have to re-assign the values. OnPropertyChanged() is not enough. (converter)
            EntitiesDefinitions = new ObservableCollection<EntityDefinition>(_entOptions.Value.Definitions);
            
            _uiOptions.Value.Language = culture.Name;
            _uiOptions.Update(_uiOptions.Value, false);
        }

        private Dictionary<Guid, DateTime> _watcherLastHostNotify = new Dictionary<Guid, DateTime>();
        private Dictionary<Guid, DateTime> _watcherLastMapChangedNotify = new Dictionary<Guid, DateTime>();
        private Dictionary<Guid, DateTime> _watcherLastPlayerSlotNotify = new Dictionary<Guid, DateTime>();
        private Dictionary<Guid, DateTime> _watcherLastAutoJoin = new Dictionary<Guid, DateTime>();
        //private System.Media.SoundPlayer _watcherNotifySoundPlayer = new System.Media.SoundPlayer();
        NAudio.Wave.IWavePlayer _watcherNotifySoundPlayer = new NAudio.Wave.WaveOut();
        // private Dictionary<Guid, int> _watcherNotifySoundPlayer_played_times = new Dictionary<Guid, int>();
        private void HandleWatcherCheck(object? sender, EventArgs e)
        {
            var cts = new CancellationTokenSource();

            // WatcherLoadingGif = WatcherGifUri_Wakeup;
            WatcherTimerHandlerStateChanged?.Invoke(this, WatcherTimerHandlerType.Start);

            Task.Run(async () =>
            {
                try
                {


                    for (int i = 0; i < _watcherOptions.Value.List.Count; i++)
                    {
                        var wItem = _watcherOptions.Value.List[i];
                        int wItem_index = i;
                        if (!wItem.IsEnabled)
                        {
                            continue;
                        }

                        string host = wItem.Host;
                        SteamQueryNet.Models.ServerInfo qInfo = null;

                        if (!string.IsNullOrEmpty(wItem.SearchName))
                        {
                            // Try to use existing host first. If cannot reach it, then search for the host.
                            if (!string.IsNullOrEmpty(host))
                            {
                                var sq = await GameQueryExtension.CreateServerQueryInstanceAsync(host);
                                qInfo = await sq.GetServerInfoAsync(cts.Token);
                            }
                            // If still cannot reach the host, then search for the host.
                            if (qInfo == null)
                            {
                                // Attempt to search for the host
                                var ipFilter = new IpFilter()
                                {
                                    AppId = QueryMaster.Game.CounterStrike_Global_Offensive,
                                    HostName = wItem.SearchName,
                                    IsDedicated = true
                                };
                                if (wItem.SearchTag != null && wItem.SearchTag.Trim() != string.Empty)
                                {
                                    ipFilter.Tags = wItem.SearchTag;
                                }

                                var sv = QueryMaster.MasterServer.MasterQuery.GetServerInstance(QueryMaster.MasterServer.MasterQuery.SourceServerEndPoint);
                                sv.GetAddresses(wItem.SearchRegion, batchInfo =>
                                {
                                    var listOfIps = batchInfo.ReceivedEndpoints.Where(x => x.Port == wItem.SearchPort).ToList();
                                    if (listOfIps.Count > 0)
                                    {
                                        host = listOfIps.First().ToString();
                                        _watcherOptions.Value.List[wItem_index].Host = host; // Save it back
                                        WatcherItems = new ObservableCollection<WatcherItem>(_watcherOptions.Value.List);
                                        _watcherOptions.Update(_watcherOptions.Value, false);
                                    }
                                }, ipFilter, batchCount: -1, err =>
                                {

                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(host))
                        {
                            if (qInfo == null)
                            {
                                var sq = await GameQueryExtension.CreateServerQueryInstanceAsync(host);    
                                qInfo = await sq.GetServerInfoAsync(cts.Token);
                            }
                            // If still cannot reach the host, then skip this watcher item.
                            if (qInfo == null)
                            {
                                continue;
                            }
                            var newInfoItem = new ServerStat()
                            {
                                SessionId = App.Current.SessionId,
                                DemandingWatcherId = wItem.Id,
                                Map = qInfo.Map,
                                MaxPlayers = qInfo.MaxPlayers,
                                Players = qInfo.Players,
                                Timestamp = DateTime.Now
                            };
                            var lastInfoItem = await _watcherRepository.GetLatestServerStatAsync(wItem.Id, App.Current.SessionId);

                            await _watcherRepository.UpsertServerStatAsync(newInfoItem);
                            if (!_watcherLastHostNotify.TryGetValue(wItem.Id, out DateTime lastHostNotify))
                            {
                                lastHostNotify = DateTime.MinValue;
                            }
                            if (!_watcherLastMapChangedNotify.TryGetValue(wItem.Id, out DateTime lastMapChangedNotify))
                            {
                                lastMapChangedNotify = DateTime.MinValue;
                            }
                            if (!_watcherLastPlayerSlotNotify.TryGetValue(wItem.Id, out DateTime lastPlayerSlotNotify))
                            {
                                lastPlayerSlotNotify = DateTime.MinValue;
                            }
                            if (!_watcherLastAutoJoin.TryGetValue(wItem.Id, out DateTime lastAutoJoin))
                            {
                                lastAutoJoin = DateTime.MinValue;
                            }
                            // Host Availability - Notify
                            if (newInfoItem.Timestamp - lastHostNotify > TimeSpan.FromSeconds(WatcherNotifyCooldown))
                            {
                                _watcherLastHostNotify[wItem.Id] = newInfoItem.Timestamp; // Upsert

                                if (wItem.IsNotifyWhenHostAvailable)
                                {
                                    if (wItem.IsNotifyViaWindowsNotification)
                                    {
                                        new ToastContentBuilder()
                                            //.AddArgument("action", "viewConversation")
                                            //.AddArgument("conversationId", 9813)
                                            .AddCustomTimeStamp(newInfoItem.Timestamp)
                                            .AddText($"{wItem.DisplayName}")
                                            .AddText("The server is online now!")
                                            .Show();
                                    }
                                    if (wItem.IsNotifyPlaySound)
                                    {
                                        HandleNotifySound(wItem);
                                    }
                                    if (wItem.IsNotifyViaDiscordWebhook)
                                    {
                                        HandleNotifyDiscordWebhook(wItem, "The server is online now!");
                                    }
                                }
                            }
                            // Host Availability - Auto Join
                            if (wItem.IsJoinWhenHostAvailable && newInfoItem.Timestamp - lastAutoJoin > TimeSpan.FromSeconds(WatcherNotifyCooldown))
                            {
                                _watcherLastAutoJoin[wItem.Id] = newInfoItem.Timestamp; // Upsert
                                JoinGame(wItem.Host);
                            }

                            // Map Changed - Notify
                            if (lastInfoItem != null && lastInfoItem.Map != newInfoItem.Map && newInfoItem.Timestamp - lastMapChangedNotify > TimeSpan.FromSeconds(WatcherNotifyCooldown))
                            {
                                _watcherLastMapChangedNotify[wItem.Id] = newInfoItem.Timestamp; // Upsert

                                if (wItem.IsNotifyWhenMapChanged)
                                {
                                    if (wItem.IsNotifyViaWindowsNotification)
                                    {
                                        new ToastContentBuilder()
                                            .AddCustomTimeStamp(newInfoItem.Timestamp)
                                            .AddText($"{wItem.DisplayName}")
                                            .AddText($"Map changed to {newInfoItem.Map}")
                                            .Show();
                                    }
                                    if (wItem.IsNotifyPlaySound)
                                    {
                                        HandleNotifySound(wItem);
                                    }
                                    if (wItem.IsNotifyViaDiscordWebhook)
                                    {
                                        HandleNotifyDiscordWebhook(wItem, $"Map changed to {newInfoItem.Map}");
                                    }
                                }
                            }

                            // Player Slot Availability - Notify
                            if (newInfoItem.Timestamp - lastPlayerSlotNotify > TimeSpan.FromSeconds(WatcherNotifyCooldown))
                            {
                                if (wItem.IsNotifyWhenSlotAvailable)
                                {
                                    if ((lastInfoItem == null || (lastInfoItem.Players >= lastInfoItem.MaxPlayers)) && newInfoItem.Players < newInfoItem.MaxPlayers)
                                    {
                                        _watcherLastPlayerSlotNotify[wItem.Id] = newInfoItem.Timestamp; // Upsert

                                        if (wItem.IsNotifyViaWindowsNotification)
                                        {
                                            new ToastContentBuilder()
                                                .AddCustomTimeStamp(newInfoItem.Timestamp)
                                                .AddText($"{wItem.DisplayName}")
                                                .AddText($"Player slot changed to {newInfoItem.Players}/{newInfoItem.MaxPlayers}")
                                                .Show();
                                        }
                                        if (wItem.IsNotifyPlaySound)
                                        {
                                            HandleNotifySound(wItem);
                                        }
                                        if (wItem.IsNotifyViaDiscordWebhook)
                                        {
                                            HandleNotifyDiscordWebhook(wItem, $"Player slot changed to {newInfoItem.Players}/{newInfoItem.MaxPlayers}");
                                        }
                                    }
                                }

                            }
                            // Player Slot Availability - Auto Join
                            if (wItem.IsJoinWhenSlotAvailable && lastInfoItem != null && newInfoItem.Timestamp - lastAutoJoin > TimeSpan.FromSeconds(WatcherNotifyCooldown))
                            {
                                _watcherLastAutoJoin[wItem.Id] = newInfoItem.Timestamp; // Upsert
                                JoinGame(host);
                            }

                            // _dataOptions.Update(_dataOptions.Value, false);

                            // var qq = await GameQueryExtension.CreateServerQueryInstanceAsync("103.219.30.229:27205");
                            // var info = await qq.GetServerInfoAsync(cts.Token);

                            //foreach (var wItem in _watcherOptions.Value.List)
                            //{
                            //    if (!wItem.IsEnabled)
                            //    {
                            //        continue;
                            //    }
                            //    var sv = QueryMaster.MasterServer.MasterQuery.GetServerInstance(QueryMaster.MasterServer.MasterQuery.SourceServerEndPoint);
                            //    sv.GetAddresses(QueryMaster.MasterServer.Region.Asia, batchInfo =>
                            //    {
                            //        string ffff = wItem.SearchName;
                            //        string ddd = ffff;
                            //    },
                            //     new IpFilter() { AppId = QueryMaster.Game.CounterStrike_Global_Offensive, HostName = wItem.SearchName, IsDedicated = true }, -1,
                            //     (ex) =>
                            //     {
                            //         if (ex != null)
                            //         {
                            //             //callback(null, ex, false);
                            //             return;
                            //         }

                            //     });
                            //}


                        }
                    }
                }
                catch (Exception ex)
                {

                }
                WatcherTimerHandlerStateChanged?.Invoke(this, WatcherTimerHandlerType.Stop);
            })
            //.ContinueWith(x => 
            //{
            //    Task.Delay(2000).Wait();
            //    WatcherLoadingGif = WatcherGifUri_Idle;
            //})
            ;

            //Task.Delay(-1).GetAwaiter().GetResult();
        }

        private void JoinGame(string host)
        {
#if DEBUG
            Windows.System.Launcher.LaunchUriAsync(new Uri($"steam://advertise/730"));
            return;
#endif
            Windows.System.Launcher.LaunchUriAsync(new Uri($"steam://connect/{host}"));
            if (IsTurnOffWatcherTimerAfterJoin)
            {
                IsWatcherEnabled = false;
            }
        }

        private void HandleNotifyDiscordWebhook(WatcherItem wItem, string msg)
        {
            if (string.IsNullOrEmpty(wItem.NotifyDiscordWebhookUrl)) return;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var webhook = new DiscordWebhookClient(wItem.NotifyDiscordWebhookUrl);
                var embed = new EmbedBuilder();
                embed.Title = $"{wItem.DisplayName}";
                embed.Description = msg;
                embed.Timestamp = DateTime.Now;
                embed.Color = Discord.Color.Green;

                await webhook.SendMessageAsync(embeds: new Embed[] { embed.Build() });
            });
        }
        private void HandleNotifySound(WatcherItem wItem)
        {
            if (string.IsNullOrEmpty(wItem.NotifySoundPath) ||
                                            !File.Exists(wItem.NotifySoundPath))
            {
                System.Media.SystemSounds.Beep.Play();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //if (!_watcherNotifySoundPlayer_played_times.TryGetValue(wItem.Id, out int playedTimes))
                    //{
                    //    playedTimes = 0;
                    //}

                    var afr = new Extensions.NAudio.LoopAudioFileReader(wItem.NotifySoundPath, wItem.NotifySoundLoop ?? 0);

                    _watcherNotifySoundPlayer.Init(afr);
                    //_watcherNotifySoundPlayer.PlaybackStopped += (s, e) =>
                    //{
                    //    if (wItem.NotifySoundLoop.HasValue)
                    //    {
                    //        if (wItem.NotifySoundLoop < 0 ||
                    //        (wItem.NotifySoundLoop != 0 &&
                    //        playedTimes < wItem.NotifySoundLoop))
                    //        {
                    //            playedTimes++;
                    //            Application.Current.Dispatcher.Invoke(() =>
                    //            {
                    //                _watcherNotifySoundPlayer.Stop();
                    //                _watcherNotifySoundPlayer.Play();
                    //            });
                    //            return;
                    //        }
                    //    }
                    //};
                    _watcherNotifySoundPlayer.Stop();
                    _watcherNotifySoundPlayer.Volume = wItem.NotifySoundVolume ?? 1.0f;
                    _watcherNotifySoundPlayer.Play();
                });

                // = new System.Media.SoundPlayer(wItem.NotifySoundPath);
                //player.Play();
            }
        }

        private System.Globalization.CultureInfo _selectedCulture;

        public System.Globalization.CultureInfo SelectedCulture
        {
            get => _selectedCulture;
            set => SetProperty(ref _selectedCulture, value);
        }

        public ICommand OpenUrlCommand => new RelayCommand<string>((url) =>
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            // use launcher to open url
            Windows.System.Launcher.LaunchUriAsync(new Uri(url));
        });

        #region PathSettings
        private string _steamPath;
        private bool _isSteamPathValid;
        private string _cs2Path;
        private bool _isCS2PathValid;
        private string _cs2acfPath;
        private bool _isCS2AcfPathValid;
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
                string steamDir = IsSteamPathValid ? Path.GetDirectoryName(SteamPath) : string.Empty;
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
                string cs2Dir = IsCS2PathValid ? Path.GetDirectoryName(CS2Path) : string.Empty;

                string cs2WorkshopDir = Path.GetFullPath(Path.Combine(cs2Dir, @"..\..\..\..\workshop"));

                dialog.InitialDirectory = IsCS2PathValid ? cs2WorkshopDir : (IsSteamPathValid ? Path.GetDirectoryName(SteamPath) : "C:");
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
                            BarMessageQueue.Enqueue(Strings.SettingsTab_Msg_AutoCs2NotFound, false);
                        }
                    }
                    else
                    {
                        BarMessageQueue.Enqueue(Strings.SettingsTab_Msg_AutoSteamNotFound, false);
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
                                BarMessageQueue.Enqueue(Strings.SettingsTab_Msg_AutoWorkshopAcfNotFound);
                            }
                        }
                        else
                        {
                            BarMessageQueue.Enqueue(Strings.SettingsTab_Msg_AutoSteamNotFound, false);
                        }
                    }
                }
                else
                {
                    BarMessageQueue.Enqueue(Strings.SettingsTab_Msg_AutoCs2NotFound, false);
                }
            }
            else
            { }
        }

        #endregion

        #region Extract
        private ObservableCollection<EntityDefinition> _entitiesDef;
        public ObservableCollection<EntityDefinition> EntitiesDefinitions { get => _entitiesDef; set => SetProperty(ref _entitiesDef, value); }

        private EntityDefinition _selectedEntity;
        public EntityDefinition SelectedEntity { get => _selectedEntity; set => SetProperty(ref _selectedEntity, value); }

        private bool _isExtracting = false;
        public bool IsExtracting { get => _isExtracting; set => SetProperty(ref _isExtracting, value); }
        private bool _isExtractConfirmDialogShown;

        public bool IsExtractConfirmDialogShown
        {
            get => _isExtractConfirmDialogShown;
            set => SetProperty(ref _isExtractConfirmDialogShown, value);
        }

        private SnackbarMessageQueue _barMessageQueue;
        public SnackbarMessageQueue BarMessageQueue { get => _barMessageQueue; set => SetProperty(ref _barMessageQueue, value); }

        private int _extractingProgressNow;
        public int ExtractingProgressNow
        {
            get => _extractingProgressNow;
            set
            {
                SetProperty(ref _extractingProgressNow, value);
                OnPropertyChanged(nameof(ExtractingProgressPercentage));
            }
        }
        private int _extractingProgressMax;
        public int ExtractingProgressMax { get => _extractingProgressMax; set => SetProperty(ref _extractingProgressMax, value); }
        private int _extractingProgressMin;
        public int ExtractingProgressMin { get => _extractingProgressMin; set => SetProperty(ref _extractingProgressMin, value); }
        public int ExtractingProgressPercentage => (int)((float)ExtractingProgressNow / (float)ExtractingProgressMax * 100);
        private DateTimeOffset? _extractingVpkPublishedDate;

        public DateTimeOffset? ExtractingVpkPublishedDate
        {
            get => _extractingVpkPublishedDate;
            set => SetProperty(ref _extractingVpkPublishedDate, value);
        }
        private string _extractingVpkTitle;

        public string ExtractingVpkTitle
        {
            get => _extractingVpkTitle;
            set => SetProperty(ref _extractingVpkTitle, value);
        }

        [RelayCommand]
        public void UpdateEntList()
        {
            Task.Run(async () => 
            {
                await UpdateEntitiesListAsync(showInfoMessage: true, showErrorMessage: true);
            });
        }
        private async Task UpdateEntitiesListAsync(bool showInfoMessage, bool showErrorMessage)
        {
            if (_advOptions.Value.EntitiesUpdateEndpoints.Length < 0)
            {
                if (showErrorMessage)
                {
                    BarMessageQueue.Enqueue($"No endpoints set, please check {nameof(AdvancedOptions)} ({AdvancedOptions.DefaultFileName})", true);
                }
                return;
            }
            try
            {
                foreach (var endpoint in _advOptions.Value.EntitiesUpdateEndpoints)
                {
                    // HttpClient
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(endpoint);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            var cfgB = new ConfigurationBuilder();
                            cfgB.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));
                            var cfg = cfgB.Build();
                            EntitiesOptions newOpt = new();
                            cfg.GetSection("entities").Bind(newOpt);
                            if (newOpt.EntVersion > _entOptions.Value.EntVersion)
                            {
                                _entOptions.Update(newOpt, false); // write to file
                                _entOptions.Value.EntVersion = newOpt.EntVersion;
                                _entOptions.Value.Definitions = newOpt.Definitions;
                                _entOptions.Value.MinVersionRequired = newOpt.MinVersionRequired;

                                EntitiesDefinitions = new ObservableCollection<EntityDefinition>(newOpt.Definitions);
                                foreach (var ent in EntitiesDefinitions)
                                {
                                    if (ent.IsDefault.HasValue && ent.IsDefault == true)
                                    {
                                        SelectedEntity = ent;
                                        break;
                                    }
                                }
                                OnPropertyChanged(nameof(SelectedEntity));
                                if (showInfoMessage)
                                {
                                    BarMessageQueue.Enqueue($"Entities list updated to version {newOpt.EntVersion}", false);
                                }
                                return;
                            }
                        }
                    }
                }
                if (showInfoMessage)
                {
                    BarMessageQueue.Enqueue($"No new version found.", false);
                }
            }
            catch (Exception ex)
            {
                if (showErrorMessage)
                {
                    BarMessageQueue.Enqueue($"Error: {ex.Message}", true);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanExtract))]
        public async Task Extract()
        {
            if (SelectedEntity == null)
            {
                BarMessageQueue.Enqueue(Strings.ExtractingTab_Msg_NoEntitySelected, true);
                return;
            }
            if (!IsSteamPathValid)
            {
                BarMessageQueue.Enqueue(Strings.ExtractingTab_Msg_SteamPathEmpty, true);
                return;
            }
            if (!IsCS2PathValid)
            {
                BarMessageQueue.Enqueue(Strings.ExtractingTab_Msg_Cs2PathEmpty, true);
                return;
            }
            if (!IsCS2AcfPathValid)
            {
                BarMessageQueue.Enqueue(Strings.ExtractingTab_Msg_WorkshopAcfPathEmpty, true);
                return;
            }

            string workshopDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(CS2AcfPath), @"content\730"));
            string vpkPath = Path.Combine(workshopDir, $"{SelectedEntity.WorkshopId}\\{SelectedEntity.WorkshopId}.vpk");
            string publishInfoTxtPath = Path.Combine(workshopDir, $"{SelectedEntity.WorkshopId}\\publish_data.txt");

            // Optionally verify hashes and signatures of the file if there are any
            // package.VerifyHashes();
            string destFolder = Path.GetFullPath(Path.Combine(CS2Path, $"../../../../game/csgo"));
            if (!Directory.Exists(destFolder))
            {
                BarMessageQueue.Enqueue($"{Strings.ExtractingTab_Msg_Cs2ActualRootDirNotFoundError} ~ {destFolder}", true);
                return;
            }
            if (!File.Exists(vpkPath))
            {
                BarMessageQueue.Enqueue($"{Strings.ExtractingTab_Msg_VpkNotFoundError} ~ {vpkPath}", true);
                return;
            }

            if (File.Exists(publishInfoTxtPath))
            {
                DateTimeOffset? dt = null;
                string title = string.Empty;
                try
                {
                    var stream = File.OpenRead(publishInfoTxtPath);
                    var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
                    KVObject data = kv.Deserialize(stream);
                    title = data["title"].ToString();
                    if (long.TryParse(data["publish_time"].ToString(), out long pubTick))
                    {
                        dt = DateTimeOffset.FromUnixTimeSeconds(pubTick);
                    }
                    else
                    {
                        dt = null;
                    }
                    
                }
                catch (Exception)
                {}
                finally
                {
                    ExtractingVpkPublishedDate = dt;
                    ExtractingVpkTitle = title;
                }
            }

            VpkExtractDestination = destFolder;
            PathVpkToBeExtracted = vpkPath;
            IsExtractConfirmDialogShown = true;


            // Extract filtered files to directory "J:\Test"
            // filtered rules are defined in SelectedEntity.Copy (List)




            //// Find a file, this returns a PackageEntry
            //var file = package.Di.FindEntry("botprofile.db");

            //if (file == null)
            //{
            //    string msg = "Cannot find botprofile.db inside this VPK.";
            //    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}
            //// Read a file to a byte array
            //package.ReadEntry(file, out byte[] fileContents);

            ////read UTF8 bytes[] into string
            //var rawText = Encoding.UTF8.GetString(fileContents);
            //RawText = rawText;
        }

        private bool CanExtract()
        {
             return !IsExtracting;
        }

        [RelayCommand(CanExecute = nameof(CanExtract))]
        public void ExtractConfirmYes()
        {
            IsExtractConfirmDialogShown = false;
            Task.Run(() =>
            {
                //FileStream fs = new FileStream(vpkPath, FileMode.Open, FileAccess.Read);
                ExtractSpecificFilesFromVpk(PathVpkToBeExtracted, SelectedEntity, VpkExtractDestination);
            });
        }
        [RelayCommand]
        public void ExtractConfirmNo() => IsExtractConfirmDialogShown = false;
        private string _vpkPathToBeExtracted;

        public string PathVpkToBeExtracted
        {
            get => _vpkPathToBeExtracted;
            set => SetProperty(ref _vpkPathToBeExtracted, value);
        }
        private string _vpkExtractDestination;

        public string VpkExtractDestination
        {
            get => _vpkExtractDestination;
            set => SetProperty(ref _vpkExtractDestination, value);
        }



        //public ICommand ExtractCommand => new RelayCommand(() =>
        //{

        //}, () => !IsExtracting);

        private void ExtractSpecificFilesFromVpk(string filepath, EntityDefinition ent, string dest)
        {
            IsExtracting = true;
            //ExtractCommand.CanExecute(false);
            using var mainPackage = new SteamDatabase.ValvePak.Package();
            //mainPackage.SetFileName(filestream.)
            mainPackage.Read(filepath);
            // Get file count without looping and filtering
            int fileCountTotal = mainPackage.Entries.Sum(x => x.Value.Count);

            var nestFileTypes = ent.Nested.Select(x => Path.GetExtension(x)).ToHashSet();

            //Dictionary<string, List<ValvePackageEntry>> dictEntriesType =
            //    mainPackage.Entries.ToDictionary(
            //        entry => entry.Key,
            //        entry => entry.Value.Select(pe => pe as ValvePackageEntry).ToList()
            //    );

            ExtractingProgressMax = fileCountTotal;
            ExtractingProgressMin = 0;
            ExtractingProgressNow = 0;

            if (ent.DeleteExplicitly != null && ent.DeleteExplicitly.Length > 0)
            {
                foreach (var relativePath in ent.DeleteExplicitly)
                {
                    string fullpath = Path.GetFullPath(Path.Combine(dest, relativePath));
                    if (Directory.Exists(fullpath))
                    {
                        // 刪除資料夾及其內容
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(fullpath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else if (File.Exists(fullpath))
                    {
                        // 刪除一個特定的檔案
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(fullpath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else if (fullpath.Contains("*"))
                    {
                        // 刪除特定類型的所有檔案
                        var directory = Path.GetDirectoryName(fullpath);
                        var searchPattern = Path.GetFileName(fullpath);

                        foreach (var file in Directory.GetFiles(directory, searchPattern))
                        {
                            FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                    }
                }
            }

            //if (ent.IsDeleteObsolete == true)
            //{
            //    foreach (var relativePath in ent.Copies)
            //    {
            //        string fullpath = Path.GetFullPath(Path.Combine(dest, relativePath));
            //        if (Directory.Exists(fullpath))
            //        {
            //            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(fullpath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            //        }
            //    }
            //}

            bool allowAllTypes = ent.Types == null || ent.Types.Length == 0 || ent.Types.Contains("*");

            HashSet<string> copiedFiles = new();

            if (ent.Nested != null && ent.Nested.Length > 0)
            {
                foreach (var nestEntPath in ent.Nested)
                {
                    var nestEntry = mainPackage.FindEntry(nestEntPath);
                    mainPackage.ReadEntry(nestEntry, out byte[] nestFileContents);

                    using var nestPackage = new SteamDatabase.ValvePak.Package();
                    if (nestEntry == null || nestFileContents == null || nestFileContents.Length == 0)
                    {
                        continue;
                    }
                    nestPackage.SetFileName(nestEntry.FileName);
                    nestPackage.Read(new MemoryStream(nestFileContents));
                    fileCountTotal += nestPackage.Entries.Sum(x => x.Value.Count);

                    // Now we actually extract the files from the nested package
                    foreach (var kv in nestPackage.Entries)
                    {
                        string fileType = kv.Key;

                        if (!allowAllTypes && ent.Types != null && ent.Types.Length > 0 && !ent.Types.Contains(fileType.ToLower()))
                        {
                            continue;
                        }

                        var fileEntries = kv.Value;
                        foreach (var fileEntry in fileEntries)
                        {
                            foreach (var copy in ent.Copies)
                            {
                                string copy_p = copy.Trim().ToLower();
                                // Skip if entry is not in the copy list
                                if (!fileEntry.DirectoryName.StartsWith(copy_p))
                                {
                                    ExtractingProgressNow++;
                                    continue;
                                }

                                string destPath = Path.GetFullPath(Path.Combine(dest, fileEntry.DirectoryName, $"{Path.GetFileName(fileEntry.FileName)}.{fileType}"));
                                string destDir = Path.GetDirectoryName(destPath);

                                Directory.CreateDirectory(destDir);

                                nestPackage.ReadEntry(fileEntry, out byte[] fileContents);

                                // If file already exists, overwrite it
                                File.WriteAllBytes(destPath, fileContents);
                                copiedFiles.Add(destPath);
                                ExtractingProgressNow++;
                            }
                        }
                    }
                }
            }

            foreach (var kv in mainPackage.Entries)//dictEntriesType)
            {
                string fileType = kv.Key;
                if (!allowAllTypes && ent.Types != null && ent.Types.Length > 0 && !ent.Types.Contains(fileType.ToLower()))
                {
                    continue;
                }

                if (nestFileTypes.Contains(fileType))
                {
                    continue;
                }

                var fileEntries = kv.Value;

                foreach (var fileEntry in fileEntries)
                {
                    foreach (var copy in ent.Copies)
                    {
                        string copy_p = copy.Trim().ToLower();
                        // Skip if entry is not in the copy list
                        if (!fileEntry.DirectoryName.StartsWith(copy_p))
                        {
                            ExtractingProgressNow++;
                            continue;
                        }

                        string destPath = Path.GetFullPath(Path.Combine(dest, fileEntry.DirectoryName, $"{Path.GetFileName(fileEntry.FileName)}.{fileType}"));
                        string destDir = Path.GetDirectoryName(destPath);

                        Directory.CreateDirectory(destDir);

                        mainPackage.ReadEntry(fileEntry, out byte[] fileContents);

                        // If file already exists, overwrite it
                        File.WriteAllBytes(destPath, fileContents);
                        copiedFiles.Add(destPath);
                        ExtractingProgressNow++;
                    }
                }
            }

            // Delete obsolete files
            if (ent.IsDeleteObsolete == true)
            {
                foreach (var copy in ent.Copies)
                {
                    // Try to find all files in the folder
                    string fullpath = Path.GetFullPath(Path.Combine(dest, copy));
                    var files = Directory.GetFiles(fullpath, "*.*", System.IO.SearchOption.AllDirectories);
                    // If file is not in the copied list, delete it
                    foreach (var file in files)
                    {
                        if (!copiedFiles.Contains(file))
                        {
                            // Delete file
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                    }
                }
            }
            
            //ExtractCommand.CanExecute(true);
            IsExtracting = false;
        }

        //private void ExtractSpecificFilesFromVpk(string vpkPath, string[] copies, string[] types, string dest)
        //{
        //    Task.Run(() =>
        //    {
        //        if (!File.Exists(vpkPath))
        //        {
        //            return;
        //        }
        //        IsExtracting = true;
        //        ExtractCommand.CanExecute(false);
        //        using var package = new SteamDatabase.ValvePak.Package();
        //        package.Read(vpkPath);

        //        // Get file count without looping and filtering
        //        int fileCountTotal = package.Entries.Sum(x => x.Value.Count);

        //        ExtractingProgressMax = fileCountTotal;
        //        ExtractingProgressMin = 0;
        //        ExtractingProgressNow = 0;

        //        foreach (var kv in package.Entries)
        //        {
        //            string fileType = kv.Key;
        //            if (types == null || types.Length == 0 || types.Contains(fileType.ToLower()))
        //            {
        //                var fileEntries = kv.Value;
        //                foreach (var copy in copies)
        //                {
        //                    string copy_p = copy.Trim().ToLower();
        //                    foreach (var fileEntry in fileEntries)
        //                    {
        //                        // Skip if entry is not in the copy list
        //                        if (!fileEntry.DirectoryName.StartsWith(copy_p))
        //                        {
        //                            ExtractingProgressNow++;
        //                            continue;
        //                        }

        //                        string destPath = Path.GetFullPath(Path.Combine(dest, fileEntry.DirectoryName, $"{Path.GetFileName(fileEntry.FileName)}.{fileType}"));
        //                        string destDir = Path.GetDirectoryName(destPath);

        //                        if (!Directory.Exists(destDir))
        //                        {
        //                            Directory.CreateDirectory(destDir);
        //                        }

        //                        package.ReadEntry(fileEntry, out byte[] fileContents);

        //                        // If file already exists, overwrite it
        //                        File.WriteAllBytes(destPath, fileContents);
        //                        ExtractingProgressNow++;
        //                    }
        //                }
        //            }
        //        }
        //        ExtractCommand.CanExecute(true);
        //        IsExtracting = false;
        //    });
        //}
        #endregion

        #region Watcher
        // private bool _isWatcherEditDialogShown;
        public enum WatcherTimerHandlerType
        {
            None,
            Start,
            Stop
        }
        public event EventHandler<WatcherTimerHandlerType> WatcherTimerHandlerStateChanged;

        private Action _watcherActionToBeConfirmed;
        public Action WatcherActionToBeConfirmed
        {
            get => _watcherActionToBeConfirmed;
            set => SetProperty(ref _watcherActionToBeConfirmed, value);
        }
        private bool _isWatcherEditConfirmDialogShown;

        public bool IsWatcherEditConfirmDialogShown
        {
            get => _isWatcherEditConfirmDialogShown;
            set => SetProperty(ref _isWatcherEditConfirmDialogShown, value);
        }
        public ICommand WatcherConfirmCancelCmd => new RelayCommand(() =>
        {
            WatcherActionToBeConfirmed = null;
            IsWatcherEditConfirmDialogShown = false;
        });
        public ICommand WatcherConfirmOkCmd => new RelayCommand(() =>
        {
            WatcherActionToBeConfirmed?.Invoke();
            IsWatcherEditConfirmDialogShown = false;
        });
        public ICommand WatcherStopSound => new RelayCommand(() =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _watcherNotifySoundPlayer.Stop();
            });
        });

        public bool IsWatcherEditDialogShown
        {
            get => _editWatcherItemDialogVM.IsShown;
            set
            {
                _editWatcherItemDialogVM.IsShown = value;
                OnPropertyChanged("IsWatcherEditDialogShown");
                //SetProperty(ref _isWatcherEditDialogShown, value);
            }
        }

        private DispatcherTimer _dispatcherTimer;

        private bool _isWatcherEnabled;
        public bool IsWatcherEnabled
        {
            get => _isWatcherEnabled;
            set => SetProperty(ref _isWatcherEnabled, value);
        }
        private int _watcherInterval;

        public int WatcherInterval
        {
            get => _watcherInterval;
            set => SetProperty(ref _watcherInterval, value);
        }
        private int _watcherNotifyCooldown;

        public int WatcherNotifyCooldown
        {
            get => _watcherNotifyCooldown;
            set => SetProperty(ref _watcherNotifyCooldown, value);
        }
        private int _watcherAutoJoinCooldown;

        public int WatcherAutoJoinCooldown
        {
            get => _watcherAutoJoinCooldown;
            set => SetProperty(ref _watcherAutoJoinCooldown, value);
        }
        private bool _isTurnOffWatcherTimerAfterJoin;

        public bool IsTurnOffWatcherTimerAfterJoin
        {
            get => _isTurnOffWatcherTimerAfterJoin;
            set => SetProperty(ref _isTurnOffWatcherTimerAfterJoin, value);
        }



        private ObservableCollection<WatcherItem> _watcherItems;
        public ObservableCollection<WatcherItem> WatcherItems
        {
            get => _watcherItems;
            set => SetProperty(ref _watcherItems, value);
        }
        private WatcherItem _selectedWatcherItem;
        public WatcherItem SelectedWatcherItem
        {
            get => _selectedWatcherItem;
            set => SetProperty(ref _selectedWatcherItem, value);
        }

        public ICommand AddWatchItemCmd => new RelayCommand(() =>
        {
            //var dialog = new Views.Dialogs.EditWatcherItemDialog();
            //var dVm = dialog.DataContext as EditWatcherItemDialogVM;
            var dVm = _editWatcherItemDialogVM;

            if (dVm == null)
            {
                return;
            }
            dVm.SetWatcherItem(new WatcherItem() { IsEnabled = true });
            dVm.IsModifyMode = false;
            dVm.SelectedSearchRegionIndex = 4;
            IsWatcherEditDialogShown = true;

            //Dispatcher.CurrentDispatcher.Invoke(() =>
            //{
            //    DialogHost.Show(dialog, "mdMainWindowDialogHost", (object sender, DialogOpenedEventArgs args) =>
            //    { }, (object sender, DialogClosingEventArgs args) =>
            //    {
            //        if (args.Parameter is WatcherItem wItem && wItem != null)
            //        {
            //            // Already saved to option instance in EditWatcherItemDialogVM
            //            WatcherItems.Add(wItem);
            //        }
            //    });
            //});
        });

        public ICommand DelWatchItemCmd => new RelayCommand<object>((x) =>
        {
            // Type test = x.GetType();

            System.Collections.IList x_ilist = (System.Collections.IList)x;
            var items = x_ilist.Cast<WatcherItem>();
            int cnt = WatcherItems.Count();
            if (cnt <= 0)
            {
                return;
            }

            // Confirm

            WatcherActionToBeConfirmed = () =>
            {
                foreach (var item in items)
                {
                    _watcherOptions.Value.List.Remove(item);
                    // WatcherItems.Remove(item);
                }
                _watcherOptions.Update(_watcherOptions.Value, false);
                WatcherItems = new ObservableCollection<WatcherItem>(_watcherOptions.Value.List);
            };
            IsWatcherEditConfirmDialogShown = true;

            return;
        }, x =>
        {
            //if (x is not List<WatcherItem> items || items.Count <= 0)
            //{
            //    return false;
            //}

            return true;
        });

        public ICommand ModifyWatchItemCmd => new RelayCommand<WatcherItem>((wItem) =>
        {
            if (wItem == null)
            {
                return;
            }

            var dVm = _editWatcherItemDialogVM;
            if (dVm == null)
            {
                return;
            }
            dVm.SetWatcherItem(wItem);
            dVm.IsModifyMode = true;
            IsWatcherEditDialogShown = true;

            // var dialog = new Views.Dialogs.EditWatcherItemDialog();
            //var dVm = dialog.DataContext as EditWatcherItemDialogVM;
            //if (dVm == null)
            //{
            //    return;
            //}
            //dVm.SetWatcherItem(wItem);
            //dVm.IsModifyMode = true;

            //DialogHost.Show(dialog, "mdMainWindowDialogHost", (object sender, DialogOpenedEventArgs args) =>
            //{
            //    //dialog.DataContext = new EditWatcherItemDialogVM(_watcherOptions);
            //}, (object sender, DialogClosingEventArgs args) =>
            //{
            //    if (args.Parameter is WatcherItem wItem && wItem != null)
            //    {
            //        // Already saved to option instance in EditWatcherItemDialogVM
            //        WatcherItems = new ObservableCollection<WatcherItem>(_watcherOptions.Value.List);
            //    }
            //});
        });

        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedCulture")
            {
                LocalizeDictionary.Instance.Culture = SelectedCulture;
            }
            #region Settings
            else if (e.PropertyName == "SteamPath")
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
            else if (
                e.PropertyName == "IsSteamPathValid" ||
                e.PropertyName == "IsCS2PathValid" ||
                e.PropertyName == "IsCS2AcfPathValid")
            {
                if (IsSteamPathValid && IsCS2PathValid && IsCS2AcfPathValid)
                {
                    // Save to config.jsonc
                    //_dirOptions.Value.Steam = SteamPath;
                    //_dirOptions.Value.Cs2 = CS2Path;
                    //_dirOptions.Value.Cs2acf = CS2AcfPath;

                    var pathOpt = new PathOptions
                    {
                        Cs2 = CS2Path,
                        Cs2acf = CS2AcfPath,
                        Steam = SteamPath
                    };

                    if (pathOpt.Steam != _dirOptions.Value.Steam ||
                        pathOpt.Cs2 != _dirOptions.Value.Cs2 ||
                        pathOpt.Cs2acf != _dirOptions.Value.Cs2acf)
                    {
                        _dirOptions.Update(pathOpt, false);
                        BarMessageQueue.Enqueue(Strings.TabSettings_Msg_SettingsSaved, false);
                    }
                }
                else
                {
                    // do nothing
                }
            }
            #endregion
            #region Extract
            else if (e.PropertyName == nameof(IsExtracting))
            {
                if (IsExtracting)
                {
                    BarMessageQueue.Enqueue(Strings.ExtractingTab_Msg_WorkStarted, false);
                }
                else
                {
                    BarMessageQueue.Enqueue(Strings.ExtractingTab_Msg_WorkFinished, false);
                    ExtractingProgressNow = 0;
                }
            }
            #endregion
            #region Watcher
            else if (e.PropertyName == nameof(IsWatcherEnabled))
            {
                _watcherOptions.Value.IsEnabled = IsWatcherEnabled;
                if (IsWatcherEnabled)
                {
                    _dispatcherTimer.Start();
                }
                else
                {
                    _dispatcherTimer.Stop();
                }
                _watcherOptions.Update(_watcherOptions.Value, false);
            }
            else if (e.PropertyName == nameof(WatcherNotifyCooldown))
            {
                _watcherOptions.Value.NotifyCooldown = WatcherNotifyCooldown;
                _watcherOptions.Update(_watcherOptions.Value, false);
            }
            else if (e.PropertyName == nameof(WatcherAutoJoinCooldown))
            {
                _watcherOptions.Value.AutoJoinCooldown = WatcherAutoJoinCooldown;
                _watcherOptions.Update(_watcherOptions.Value, false);
            }
            else if (e.PropertyName == nameof(IsTurnOffWatcherTimerAfterJoin))
            {
                _watcherOptions.Value.IsTurnOffTimerAfterJoin = IsTurnOffWatcherTimerAfterJoin;
                _watcherOptions.Update(_watcherOptions.Value, false);
            }
            else if (e.PropertyName == nameof(WatcherInterval))
            {
                _watcherOptions.Value.Interval = WatcherInterval;
                _dispatcherTimer.Interval = TimeSpan.FromSeconds(WatcherInterval);
                _watcherOptions.Update(_watcherOptions.Value, false);
            }
            #endregion
            base.OnPropertyChanged(e);
        }


    }
}
