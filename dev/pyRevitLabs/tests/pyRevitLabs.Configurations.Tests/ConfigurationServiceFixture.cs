using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using pyRevitLabs.Configurations.Abstractions;

namespace pyRevitLabs.Configurations.Tests
{
    public sealed class ConfigurationServiceFixture : IDisposable
    {
        public ConfigurationServiceFixture()
        {
            Configuration = new ConfigurationService(new List<IConfiguration>() {new TestRunConfiguration()});
        }

        public IConfiguration Configuration { get; }

        public void Dispose() { }

        private class TestRunConfiguration : IConfiguration
        {
            public bool HasSection(string sectionName)
            {
                throw new NotImplementedException();
            }

            public bool HasSectionKey(string sectionName, string keyName)
            {
                throw new NotImplementedException();
            }

            public T GetValue<T>(string sectionName, string keyName)
            {
                throw new NotImplementedException();
            }

            public T? GetValueOrDefault<T>(string sectionName, string keyName, T? defaultValue = default)
            {
                throw new NotImplementedException();
            }

            public bool RemoveValue(string sectionName, string keyName)
            {
                throw new NotImplementedException();
            }

            public void SetValue<T>(string sectionName, string keyName, T? value)
            {
                throw new NotImplementedException();
            }
        }
    }
}