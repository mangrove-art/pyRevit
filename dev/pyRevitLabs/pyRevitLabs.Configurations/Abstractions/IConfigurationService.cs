﻿namespace pyRevitLabs.Configurations.Abstractions;

public interface IConfigurationService
{
    CoreSection Core { get; }
    RoutesSection Routes { get; }
    TelemetrySection Telemetry { get; }

    T GetSection<T>();
    void SaveSection<T>(string configurationName, T sectionValue);

    void SetSectionKeyValue<T>(string configurationName, string sectionName, string keyName, T keyValue);
    T? GetSectionKeyValueOrDefault<T>(string configurationName, string sectionName, string keyName, T? defaultValue = default);
}