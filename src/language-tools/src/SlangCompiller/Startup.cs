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
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "Settings.json"))
                .Build();
        }

        public IServiceProvider ConfigureServices()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddCompillers();

            serviceCollection.Configure<CppCompillerOptions>(o =>
            {
                string path = _configuration
                    .GetSection(nameof(CppCompillerOptions))
                    .GetValue<string>(nameof(CppCompillerOptions.CppCompillerPath));

                if (path.StartsWith("~/"))
                {
                    path = Path.Combine(AppContext.BaseDirectory, path.Substring(2));
                }

                o.CppCompillerPath = path;
            });

            return serviceCollection.BuildServiceProvider();
        }
    }
}