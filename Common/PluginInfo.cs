using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

public class PluginInfo {
    public string fullName;
    public string shortName;
    public string description;
    public string authorName;
    public string version;

    public PluginInfo() {
        var assembly = typeof(PluginInfo).Assembly;
        var assemblyName = assembly.GetName();

        var descriptionAttr = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
        var copyrightAttr = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
        var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();

        fullName = titleAttribute.Title ?? string.Empty;
        shortName = assemblyName.Name ?? string.Empty;
        description = descriptionAttr?.Description ?? string.Empty;
        version = assemblyName.Version?.ToString() ?? string.Empty;

        var copyright = copyrightAttr?.Copyright;
        if (copyright != null) {
            var match = Regex.Match(copyright, @"©\s*(.*?)(?=\s*,?\s*\d{4}|$)");
            authorName = match.Success ? match.Groups[1].Value : string.Empty;
        } else {
            authorName = string.Empty;
        }
    }
}
