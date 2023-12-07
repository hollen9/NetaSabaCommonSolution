using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using NetaSabaPortal.Models;

namespace NetaSabaPortal.Options;


/// <summary>
/// "watcher": {
///  "isEnabled": true,
///  "interval": 30000,
///  "list": [
///    {
///      "name": "",
///      "isEnabled": true,
///      "host": "",
///      "password": "",
///      "isJoinWhenHostAvailable": true,
///      "isJoinWhenSlotAvailable": true,
///      "isNotifyWhenHostAvailable": true,
///      "isNotifyWhenSlotAvailable": true,
///      "isNotifyWhenMapChanged": true
///    }
///  ]
///}
/// </summary>
public class WatcherOptions
{
    public const string DefaultFileName = "config_watcher.json";
    private List<WatcherItem> _list;

    public bool IsEnabled { get; set; }
    public int Interval { get; set; }
    public List<WatcherItem> List { get => _list; set => _list = value; }
    public WatcherOptions() { }
    public WatcherOptions(bool isEnabled, int interval, List<WatcherItem> list)
    {
        IsEnabled = isEnabled;
        Interval = interval;
        List = list;
    }
}


