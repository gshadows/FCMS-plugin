using System;
using System.Text;
using System.IO;

public static class Logger {
    private static bool headerSent = false;
    private static string version = typeof(Logger).Assembly.GetName().Version.ToString();
    public static string logFileName = "";

    public static void log(string message) {
        if (logFileName == null || logFileName == "") return;
        string formattedMessage = $"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.s")} {message}\n";
        string ret;
        if (!headerSent) {
            ret = $"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.s")} {version}\n{formattedMessage}";
            headerSent = true;
        } else {
            ret = formattedMessage;
        }
        try {
            File.AppendAllText(logFileName, ret);
        }
        catch { }
    }
}
