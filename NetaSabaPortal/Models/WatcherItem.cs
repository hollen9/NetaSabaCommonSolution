using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Models;

public interface IWatcherItem
{
    Guid Id { get; set; }
    string DisplayName { get; set; }
    string SearchName { get; set; }
    int? SearchPort { get; set; }
    QueryMaster.MasterServer.Region SearchRegion { get; set; }
    string SearchTag { get; set; }
    bool IsEnabled { get; set; }
    string Host { get; set; }
    string Password { get; set; }
    bool IsJoinWhenHostAvailable { get; set; }
    bool IsJoinWhenSlotAvailable { get; set; }
    bool IsNotifyWhenHostAvailable { get; set; }
    bool IsNotifyWhenSlotAvailable { get; set; }
    bool IsNotifyWhenMapChanged { get; set; }
    bool IsNotifyViaDiscordWebhook { get; set; }
    string NotifyDiscordWebhookUrl { get; set; }
    bool IsNotifyViaWindowsNotification { get; set; }
    bool IsNotifyPlaySound { get; set; }
    string NotifySoundPath { get; set; }
    int? NotifySoundLoop { get; set; }
    float? NotifySoundVolume { get; set; }

}

/// <summary>
/// "displayName": "Test",
/// "searchName": "R0对战平台天梯服务器 上海36",
/// "isEnabled": true,
/// "host": "", //122.51.40.180:27015
/// "password": "",
/// "isJoinWhenHostAvailable": true,
/// "isJoinWhenSlotAvailable": true,
/// "isNotifyWhenHostAvailable": true,
/// "isNotifyWhenSlotAvailable": true,
/// "isNotifyWhenMapChanged": true
/// </summary>
public class WatcherItem : ObservableObject, IWatcherItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; }
    public string SearchName { get; set; }
    public int? SearchPort { get; set; }
    public QueryMaster.MasterServer.Region SearchRegion { get; set; }
    public string SearchTag { get; set; }
    public bool IsEnabled { get; set; }
    public string Host { get; set; }
    public string Password { get; set; }
    public bool IsJoinWhenHostAvailable { get; set; }
    public bool IsJoinWhenSlotAvailable { get; set; }
    public bool IsNotifyWhenHostAvailable { get; set; }
    public bool IsNotifyWhenSlotAvailable { get; set; }
    public bool IsNotifyWhenMapChanged { get; set; }
    public bool IsNotifyViaDiscordWebhook { get; set; }
    public string NotifyDiscordWebhookUrl { get; set; }
    public bool IsNotifyViaWindowsNotification { get; set; }
    public bool IsNotifyPlaySound { get; set; }
    public string NotifySoundPath { get; set; }
    public int? NotifySoundLoop { get; set; }
    public float? NotifySoundVolume { get; set; }

}
