namespace pyRevitLabs.Configurations.Abstractions;

public interface IConfiguration
{ 
    string ConfigurationPath { get; }
    
    bool HasSection(string sectionName);
    bool HasSectionKey(string sectionName, string keyName);

    T GetValue<T>(string sectionName, string keyName);
    T? GetValueOrDefault<T>(string sectionName, string keyName, T? defaultValue = default);

    bool RemoveValue(string sectionName, string keyName);
    void SetValue<T>(string sectionName, string keyName, T? value);

    void SaveConfiguration();
    void SaveConfiguration(string configurationPath);
}