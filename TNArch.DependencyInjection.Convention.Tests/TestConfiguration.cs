using System;
using TNArch.DependencyInjection.Convention.Abstractions;

namespace TNArch.DependencyInjection.Convention.Tests
{
    [ConfigurationDescriptor("Settings:TestConfigs")]
    public class TestConfiguration
    {
        public string Value1 { get; set; }
        public TimeSpan Value2 { get; set; }
    }
}
