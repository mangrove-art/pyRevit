using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using pyRevitLabs.Configurations.Abstractions;
using pyRevitLabs.Configurations.Attributes;
using pyRevitLabs.Configurations.Exceptions;

namespace pyRevitLabs.Configurations;

public sealed class ConfigurationService : IConfigurationService
{
    private readonly List<ConfigurationName> _names;
    private readonly IDictionary<string, IConfiguration> _configurations;

    public static readonly string DefaultConfigurationName = "Default";

    internal ConfigurationService(
        List<ConfigurationName> names,
        IDictionary<string, IConfiguration> configurations)
    {
        _names = names;
        _configurations = configurations;
    }

    internal static IConfigurationService Create(
        List<ConfigurationName> names,
        IDictionary<string, IConfiguration> configurations)
    {
        return new ConfigurationService(names, configurations);
    }

    public IEnumerable<string> ConfigurationNames => _configurations.Keys;

    public IEnumerable<IConfiguration> Configurations => _names
        .Select(item => _configurations[item.Name!])
        .ToArray();

    public CoreSection? Core { get; private set; }
    public RoutesSection? Routes { get; private set; }
    public TelemetrySection? Telemetry { get; private set; }
    
    public void LoadConfigurations()
    {
        Core = GetSection<CoreSection>();
        Routes = GetSection<RoutesSection>();
        Telemetry = GetSection<TelemetrySection>();
    }

    public void SaveSection<T>(string configurationName, T sectionValue)
    {
        if (sectionValue is null)
            throw new ArgumentNullException(nameof(sectionValue));

        if (string.IsNullOrWhiteSpace(configurationName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(configurationName));

        if (!_configurations.TryGetValue(configurationName, out IConfiguration? configuration))
            throw new ArgumentException($"Configuration with name {configurationName} not found");
        
        Type configurationType = typeof(T);
        SaveSection(configurationType, sectionValue, configuration);
    }

    private static void SaveSection(Type configurationType, object? sectionValue, IConfiguration configuration)
    {
        string sectionName =
            GetCustomAttribute<SectionNameAttribute>(configurationType)?.SectionName ?? configurationType.Name;

        foreach (var propertyInfo in configurationType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            string keyName = GetCustomAttribute<KeyNameAttribute>(propertyInfo)?.KeyName ?? propertyInfo.Name;
            object? keyValue = propertyInfo.GetValue(sectionValue);
            if (keyValue is not null)
                configuration.SetValue(sectionName, keyName, keyValue);
        }
    }

    internal T GetSection<T>()
    {
        Type configurationType = typeof(T);
        return (T) CreateSection(configurationType, Configurations.ToArray());
    }

    private static object CreateSection(Type configurationType, params IConfiguration[] configurations)
    {
        string sectionName =
            GetCustomAttribute<SectionNameAttribute>(configurationType)?.SectionName ?? configurationType.Name;

        var sectionConfiguration = Activator.CreateInstance(configurationType);

        foreach (var propertyInfo in configurationType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            string keyName = GetCustomAttribute<KeyNameAttribute>(propertyInfo)?.KeyName ?? propertyInfo.Name;

            object? keyValue = GetKeyValue(configurations, propertyInfo, sectionName, keyName);
            propertyInfo.SetValue(sectionConfiguration, keyValue ?? propertyInfo.GetValue(sectionConfiguration));
        }

        return sectionConfiguration!;
    }

    private static object? GetKeyValue(
        IEnumerable<IConfiguration> configurations,
        PropertyInfo propertyInfo,
        string sectionName, string keyName)
    {
        return configurations
            .Select(item=> item.GetValueOrDefault(propertyInfo.PropertyType, sectionName, keyName))
            .FirstOrDefault(item => item != default);
    }

    private static T? GetCustomAttribute<T>(MemberInfo memberInfo) where T : Attribute
    {
        return memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
    }
}