using Observatory.Framework;
using System.Text.Json.Serialization;
using System;
using System.IO;

class FcmsSettings {
    public static readonly string DEFAULT_LOG_NAME = "FCMS.log";
    
    public static readonly FcmsSettings DEFAULT = new() {
        userName = "",
        apiKey = "",
        LogFile = DEFAULT_LOG_NAME,
    };


    [SettingDisplayName("User Name")]
    public string userName { get; set; }

    [SettingDisplayName("API Key")]
    public string apiKey { get; set; }


    [SettingDisplayName("Log File")]
    [System.Text.Json.Serialization.JsonIgnore]
    public FileInfo LogFileLocation { get => new FileInfo(LogFile); set => LogFile = value.FullName; }

    [SettingIgnore]
    public string LogFile { get; set; }
}
