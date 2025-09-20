using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EDDDLLInterfaces.EDDDLLIF;

public class FcmsEDDClass {
    private EDDCallBacks eddCallbacks;
    
    private UniqueInstanceChecker uniqueInstanceChecker = new UniqueInstanceChecker();
    private PluginInfo pluginInfo = new PluginInfo();
    private FcmsApi api;
    private Settings settings = new Settings("");
    
    
    public FcmsEDDClass() {
        Logger.logFileName = Path.Combine(Path.GetTempPath(), "FCMS.log");
        Logger.log("FcmsEDDClass instantiated");
    }
    
    public string EDDInitialise(string vstr, string dllfolder, EDDCallBacks cb) {
        eddCallbacks = cb;
        
        //if (settings.LogFile == FcmsSettings.DEFAULT_LOG_NAME) {
        //    settings.LogFile = observatoryCore.PluginStorageFolder + FcmsSettings.DEFAULT_LOG_NAME;
        //}
        Logger.logFileName = Path.Combine(dllfolder, "FCMS.log");
        
        string userAgent = $"{pluginInfo.shortName}/{pluginInfo.version}";
        api = new FcmsApi(settings.userName, settings.apiKey, userAgent, new HttpClient());
        
        var unique = uniqueInstanceChecker.isUniqueInstance();
        Logger.log($"Checking unique instance: {unique}");
        
        return pluginInfo.version;
    }
    
    
    public void EDDNewJournalEntry(JournalEntry je) {
        Logger.log($"EDDNewJournalEntry: ver={je.ver}, name={je.name}, eventid={je.eventid}, systemname={je.systemname}, bodyname={je.bodyname}");
        
        if (!uniqueInstanceChecker.isUniqueInstance()) {
            Logger.log("Skipping FCMS: another instance already running!");
            return;
        }
        
        switch (je.name) { // je.eventid ???
            case "CarrierJumpRequest":
                api.SendCarrierJumpRequest(je.systemname, je.bodyname);
                break;
            case "CarrierJumpCancelled":
                api.SendCarrierJumpCancelled();
                break;
            case "CarrierTradeOrder":
                api.SendMarketUpdate();
                break;
        }
    }
    
    
    private class Settings {
        public string userName;
        public string apiKey;
        
        public Settings(string encoded) {
            var parts = encoded.Split("|");
            userName = (parts.Length > 0) ? parts[0] : "";
            apiKey   = (parts.Length > 1) ? parts[1] : "";
        }
        
        public Settings(string userName, string apiKey) {
            this.userName = userName;
            this.apiKey   = apiKey;
        }
        
        public string encode() => $"{userName}|{apiKey}";
    }
    
    
    
    public string EDDConfig(string configStr, bool editit) {
        Logger.log($"EDDConfig: edit={editit}, configStr={configStr}");
        settings = new Settings(configStr);
        if (editit) {
            settings = showOptionsDialog(settings);
            configStr = settings.encode();
        }
        applySettings(settings);
        return configStr;
    }
    
    private void applySettings(Settings settings) {
        api.user = settings.userName;
        api.apiKey = settings.apiKey;
    }
    
    private Settings showOptionsDialog(Settings settings) {
        Form dialog = new Form() {
            Width = 500,
            Height = 300,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = "FCMS connection settings",
            StartPosition = FormStartPosition.CenterScreen
        };
        
        Label lblUserName   = new Label()   { Left =  8, Top = 20, Text = "User name:" };
        TextBox txtUserName = new TextBox() { Left = 48, Top = 20, Width = 400, Text = settings.userName };
        
        Label lblApiKey   = new Label()   { Left =  8, Top = 40, Text = "API key:" };
        TextBox txtApiKey = new TextBox() { Left = 48, Top = 40, Width = 400, Text = settings.apiKey };
        
        Button okButton = new Button() { Text = "OK", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK };
        okButton.Click += (sender, e) => { dialog.Close(); };
        
        dialog.Controls.Add(lblUserName);
        dialog.Controls.Add(txtUserName);
        dialog.Controls.Add(lblApiKey);
        dialog.Controls.Add(txtApiKey);
        dialog.Controls.Add(okButton);
        dialog.AcceptButton = okButton;
        
        if (dialog.ShowDialog() == DialogResult.OK) {
            settings.userName = txtUserName.Text;
            settings.apiKey = txtApiKey.Text;
        }
        return settings;
    }
    
    
    //public void EDDRefresh(string cmdname, JournalEntry lastje) {}
    //public void EDDMainFormShown() {}
    
    public void EDDTerminate() {
        Logger.log("EDDTerminate");
    }
}
