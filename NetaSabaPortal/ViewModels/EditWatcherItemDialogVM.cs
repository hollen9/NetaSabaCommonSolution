using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using NetaSabaPortal.Models;
using NetaSabaPortal.Options;
using Nogic.WritableOptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFLocalizeExtension.Engine;

namespace NetaSabaPortal.ViewModels
{
    public class EditWatcherItemDialogVM : ObservableObject, IWatcherItem
    {
        private readonly IWritableOptions<WatcherOptions> _watcherOptions;
        
        public EditWatcherItemDialogVM(IWritableOptions<WatcherOptions> watcherOptions)
        {
            _watcherOptions = watcherOptions;
            
            //Id = Guid.NewGuid();
            //IsEnabled = true;
            LocalizeDictionary.Instance.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == "Culture")
                {
                    HandleUiLanguageChanged(LocalizeDictionary.Instance.Culture);
                }
            };
            RegionItems = new ObservableCollection<MasterServerRegionItem>();
            HandleUiLanguageChanged(LocalizeDictionary.Instance.Culture);
            SelectedSearchRegionIndex = 4;
        }

        private void HandleUiLanguageChanged(CultureInfo culture)
        {

            // RegionItems = new ObservableCollection<QueryMaster.Region>(QueryMaster.RegionHelper.GetRegions());
            RegionItems.Clear();
            for (int i = 0; i < 256; i++)
            {
                if (i == 7 + 1)
                {
                    i = 255;
                }
                string value = LocalizeDictionary.Instance.GetLocalizedObject("_RegionItems_" + i, null, culture).ToString();
                RegionItems.Add(new MasterServerRegionItem() { Region = (QueryMaster.MasterServer.Region)i, DisplayName = value });
            }

        }

        private bool _isShown;

        public bool IsShown
        {
            get => _isShown;
            set => SetProperty(ref _isShown, value);
        }

        private bool _isModifyMode;
        public bool IsModifyMode
        {
            get => _isModifyMode;
            set => SetProperty(ref _isModifyMode, value);
        }

        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }


        

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }
        private string _searchName;
        public string SearchName
        {
            get => _searchName;
            set => SetProperty(ref _searchName, value);
        }
        private int? _searchPort;
        public int? SearchPort
        {
            get => _searchPort;
            set => SetProperty(ref _searchPort, value);
        }
        private QueryMaster.MasterServer.Region _searchRegion;

        public QueryMaster.MasterServer.Region SearchRegion
        {
            get => _searchRegion;
            set 
            {
                SetProperty(ref _searchRegion, value);
                for (int i = 0; i < RegionItems.Count; i++)
                {
                    if (RegionItems[i].Region == value)
                    {
                        if (SelectedSearchRegionIndex != i) SelectedSearchRegionIndex = i;
                        break;
                    }
                }
            }
        }

        private int _selectedSearchRegionIndex;

        public int SelectedSearchRegionIndex
        {
            get => _selectedSearchRegionIndex;
            set 
            {
                SetProperty(ref _selectedSearchRegionIndex, value);
                if (SearchRegion != RegionItems[value].Region) SearchRegion = RegionItems[value].Region;
            }
        }



        private ObservableCollection<MasterServerRegionItem> _regionItems;
        public ObservableCollection<MasterServerRegionItem> RegionItems
        {
            get => _regionItems;
            set => SetProperty(ref _regionItems, value);
        }


        private string _searchTag;

        public string SearchTag
        {
            get => _searchTag;
            set => SetProperty(ref _searchTag, value);
        }


        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
        private string _host;
        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }
        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        private bool _isJoinWhenHostAvailable;
        public bool IsJoinWhenHostAvailable
        {
            get => _isJoinWhenHostAvailable;
            set => SetProperty(ref _isJoinWhenHostAvailable, value);
        }
        private bool _isJoinWhenSlotAvailable;
        public bool IsJoinWhenSlotAvailable
        {
            get => _isJoinWhenSlotAvailable;
            set => SetProperty(ref _isJoinWhenSlotAvailable, value);
        }
        private bool _isNotifyWhenHostAvailable;
        public bool IsNotifyWhenHostAvailable
        {
            get => _isNotifyWhenHostAvailable;
            set => SetProperty(ref _isNotifyWhenHostAvailable, value);
        }
        private bool _isNotifyWhenSlotAvailable;
        public bool IsNotifyWhenSlotAvailable
        {
            get => _isNotifyWhenSlotAvailable;
            set => SetProperty(ref _isNotifyWhenSlotAvailable, value);
        }
        private bool _isNotifyWhenMapChanged;
        
        public bool IsNotifyWhenMapChanged
        {
            get => _isNotifyWhenMapChanged;
            set => SetProperty(ref _isNotifyWhenMapChanged, value);
        }

        private bool _isNotifyViaDiscordWebhook;

        public bool IsNotifyViaDiscordWebhook
        {
            get => _isNotifyViaDiscordWebhook;
            set => SetProperty(ref _isNotifyViaDiscordWebhook, value);
        }

        private string _notifyDiscordWebhookUrl;

        public string NotifyDiscordWebhookUrl
        {
            get => _notifyDiscordWebhookUrl;
            set => SetProperty(ref _notifyDiscordWebhookUrl, value);
        }

        private bool _isNotifyViaWindowsNotification;

        public bool IsNotifyViaWindowsNotification
        {
            get => _isNotifyViaWindowsNotification;
            set => SetProperty(ref _isNotifyViaWindowsNotification, value);
        }

        private bool _isNotifyPlaySound;

        public bool IsNotifyPlaySound
        {
            get => _isNotifyPlaySound;
            set => SetProperty(ref _isNotifyPlaySound, value);
        }

        private string _notifySoundPath;

        public string NotifySoundPath
        {
            get => _notifySoundPath;
            set => SetProperty(ref _notifySoundPath, value);
        }

        private int? _notifySoundLoop;

        public int? NotifySoundLoop
        {
            get => _notifySoundLoop;
            set => SetProperty(ref _notifySoundLoop, value);
        }

        private float? _notifySoundVolume;

        public float? NotifySoundVolume
        {
            get => _notifySoundVolume;
            set => SetProperty(ref _notifySoundVolume, value);
        }

        private bool _isNotifyPlaySoundLoop;

        public bool IsNotifyPlaySoundLoop
        {
            get => _isNotifyPlaySoundLoop;
            set => SetProperty(ref _isNotifyPlaySoundLoop, value);
        }


        private string _validateMessage;

        public delegate void ShownChangedHandler(object sender);
        public event ShownChangedHandler ShownChanged;
        public delegate void ItemSavedHandler<ItemSaveEventArg>(object? sender, ItemSaveEventArg e);
        public event ItemSavedHandler<ItemSaveEventArg> ItemSaved;
        public class ItemSaveEventArg : EventArgs
        {
            public ItemSaveEventArg(WatcherItem watcherItem, bool isModifyMode)
            {
                WatcherItem = watcherItem;
                IsModifyMode = isModifyMode;
            }
            public WatcherItem WatcherItem { get; set; }
            public bool IsModifyMode { get; set; }
        }

        public string ValidateMessage
        {
            get => _validateMessage;
            set => SetProperty(ref _validateMessage, value);
        }

        internal IWatcherItem SetWatcherItem(WatcherItem watcherItem, IWatcherItem dest = null)
        {
            if (dest == null)
            {
                dest = this;
            }
            dest.Id = watcherItem.Id;
            dest.DisplayName = watcherItem.DisplayName;
            dest.SearchName = watcherItem.SearchName;
            dest.SearchPort = watcherItem.SearchPort;
            dest.SearchRegion = watcherItem.SearchRegion;
            dest.SearchTag = watcherItem.SearchTag;
            dest.IsEnabled = watcherItem.IsEnabled;
            dest.Host = watcherItem.Host;
            dest.Password = watcherItem.Password;
            dest.IsJoinWhenHostAvailable = watcherItem.IsJoinWhenHostAvailable;
            dest.IsJoinWhenSlotAvailable = watcherItem.IsJoinWhenSlotAvailable;
            dest.IsNotifyWhenHostAvailable = watcherItem.IsNotifyWhenHostAvailable;
            dest.IsNotifyWhenSlotAvailable = watcherItem.IsNotifyWhenSlotAvailable;
            dest.IsNotifyWhenMapChanged = watcherItem.IsNotifyWhenMapChanged;
            dest.IsNotifyViaDiscordWebhook = watcherItem.IsNotifyViaDiscordWebhook;
            dest.NotifyDiscordWebhookUrl = watcherItem.NotifyDiscordWebhookUrl;
            dest.IsNotifyViaWindowsNotification = watcherItem.IsNotifyViaWindowsNotification;
            dest.IsNotifyPlaySound = watcherItem.IsNotifyPlaySound;
            dest.NotifySoundPath = watcherItem.NotifySoundPath;
            dest.NotifySoundLoop = watcherItem.NotifySoundLoop;
            dest.NotifySoundVolume = watcherItem.NotifySoundVolume;
            return dest;
        }
        internal WatcherItem ProduceWatcherItem()
        {
            var result = new WatcherItem()
            {
                Id = Id,
                DisplayName = DisplayName,
                SearchName = SearchName,
                SearchPort = SearchPort,
                SearchRegion = SearchRegion,
                SearchTag = SearchTag,
                IsEnabled = IsEnabled,
                Host = Host,
                Password = Password,
                IsJoinWhenHostAvailable = IsJoinWhenHostAvailable,
                IsJoinWhenSlotAvailable = IsJoinWhenSlotAvailable,
                IsNotifyWhenHostAvailable = IsNotifyWhenHostAvailable,
                IsNotifyWhenSlotAvailable = IsNotifyWhenSlotAvailable,
                IsNotifyWhenMapChanged = IsNotifyWhenMapChanged,
                IsNotifyViaDiscordWebhook = IsNotifyViaDiscordWebhook,
                NotifyDiscordWebhookUrl = NotifyDiscordWebhookUrl,
                IsNotifyViaWindowsNotification = IsNotifyViaWindowsNotification,
                IsNotifyPlaySound = IsNotifyPlaySound,
                NotifySoundPath = NotifySoundPath,
                NotifySoundLoop = NotifySoundLoop,
                NotifySoundVolume = NotifySoundVolume,
            };
            return result;
        }

        public ICommand SaveCommand => new RelayCommand(() => 
        {
            WatcherItem watcherItem = null;
            if (IsModifyMode == false) //Adding mode
            {
                watcherItem = ProduceWatcherItem();
                Validate(watcherItem);
                _watcherOptions.Value.List.Add(watcherItem);
                _watcherOptions.Update(_watcherOptions.Value);
            }
            else //Modifying mode
            {
                var match = _watcherOptions.Value.List.Where(x => x.Id == Id).FirstOrDefault();
                if (match == null)
                {
                    throw new Exception("Saving failed.");
                }
                watcherItem = ProduceWatcherItem();
                Validate(watcherItem);

                match = SetWatcherItem(watcherItem, match) as WatcherItem;
                _watcherOptions.Update(_watcherOptions.Value);
            }
            ValidateMessage = string.Empty;
            // DialogHost.Close("mdMainWindowDialogHost", watcherItem);
            ItemSaved?.Invoke(this, new ItemSaveEventArg(watcherItem, IsModifyMode));
            IsShown = false;
        });

        private void Validate(WatcherItem watcherItem)
        {
            if (string.IsNullOrWhiteSpace(watcherItem.DisplayName))
            {
                ValidateMessage = "DisplayName cannot be empty!";
                return;
            }
            
            // Find if there is any item with the same DisplayName or Host or SearchName
            // If there is, prompt error message
            // If there is not, add the item to the list
            bool isDisplayNameExists = false;
            bool isHostExists = false;
            bool isSearchNameExists = false;
            bool isIdExists = false;
            foreach (var item in _watcherOptions.Value.List)
            {
                if (item.Id == watcherItem.Id)
                {
                    isIdExists = true;
                    break;
                }
                if (item.DisplayName == watcherItem.DisplayName)
                {
                    isDisplayNameExists = true;
                    break;
                }
                if (item.Host == watcherItem.Host)
                {
                    isHostExists = true;
                    break;
                }
                if (item.SearchName == watcherItem.SearchName)
                {
                    isSearchNameExists = true;
                    break;
                }
            }
            if (isIdExists)
            {
                ValidateMessage = "Id already exists!";
                return;
            }
            if (isDisplayNameExists)
            {
                ValidateMessage = "DisplayName already exists. Please use other name.";
                return;
            }
            if (isHostExists)
            {
                ValidateMessage = "Host already exists. Please edit it instead.";
                return;
            }
            if (isSearchNameExists)
            {
                ValidateMessage = "SearchName already exists. Please edit it instead.";
                return;
            }
        }

        public ICommand CloseCommand => new RelayCommand(() =>
        {
            // DialogHost.Close("mdMainWindowDialogHost", null);
            IsShown = false;
        });

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsShown))
            {
                ShownChanged?.Invoke(this);
            }
            base.OnPropertyChanged(e);
        }
    }
}
