using System;
using System.IO;
using CompillerServices.Cpp;
using CompillerServices.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SlangCompiller
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Settings.json")
                .Build();
        }

        public IServiceProvider ConfigureServices()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddCompillers();

            serviceCollection.Configure<CppCompillerOptions>(_configuration.GetSection(nameof(CppCompillerOptions)));

            return serviceCollection.BuildServiceProvider();
        }
    }
}