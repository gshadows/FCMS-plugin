using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using Observatory.Framework.Sorters;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class FcmsWorker : IObservatoryWorker {
    private const string GITHUB = "https://github.com/gshadows/FCMS-plugin";
    
    // Observatory Core fields.
    private IObservatoryCore core;
    private PluginUI pluginUI;
    private AboutInfo aboutInfo;
    
    // Observatory Core override properties.
    public AboutInfo AboutInfo => aboutInfo;
    public string Version => pluginInfo.version;
    public PluginUI PluginUI => pluginUI;
    public object Settings { get => settings; set { settings = (FcmsSettings)value; } }
    
    // Project fields.
    private UniqueInstanceChecker uniqueInstanceChecker = new UniqueInstanceChecker();
    private FcmsSettings settings = FcmsSettings.DEFAULT;
    private PluginInfo pluginInfo = new PluginInfo();
    private FcmsApi api;
    
    
    public FcmsWorker() {
        aboutInfo = new() {
            FullName = pluginInfo.fullName,
            ShortName = pluginInfo.shortName,
            Description = pluginInfo.description,
            AuthorName = pluginInfo.authorName,
            Links = new() {
                new AboutLink("GitHub", GITHUB),
            }
        };
    }


    public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase {
        if (core.IsLogMonitorBatchReading) return; // Skip historical events. Only realtime need.
        
        if (!uniqueInstanceChecker.isUniqueInstance()) {
            Logger.log("Skipping FCMS: another instance already running!");
            return;
        }
        
        switch (journal) {
            case CarrierJumpRequest jumpRequest:
                api.SendCarrierJumpRequest(jumpRequest.SystemName, jumpRequest.Body);
                break;
            case CarrierJumpCancelled jumpCancelled:
                api.SendCarrierJumpCancelled();
                break;
            case CarrierTradeOrder tradeOrder:
                api.SendMarketUpdate();
                break;
        }
    }

    public void Load(IObservatoryCore observatoryCore) {
        core = observatoryCore;
        pluginUI = new PluginUI(new ObservableCollection<object>());
        
        if (settings.LogFile == FcmsSettings.DEFAULT_LOG_NAME) {
            settings.LogFile = observatoryCore.PluginStorageFolder + FcmsSettings.DEFAULT_LOG_NAME;
        }
        Logger.logFileName = settings.LogFile;
        
        string userAgent = $"{pluginInfo.shortName}/{pluginInfo.version}";
        api = new FcmsApi(settings.userName, settings.apiKey, userAgent, core.HttpClient);
        
        var unique = uniqueInstanceChecker.isUniqueInstance();
        Logger.log($"Checking unique instance: {unique}");
    }

    public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args) {
    }
}
