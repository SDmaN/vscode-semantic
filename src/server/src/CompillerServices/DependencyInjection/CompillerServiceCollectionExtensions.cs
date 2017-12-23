using System;
using CompillerServices.Backend;
using CompillerServices.Backend.ProjectFile;
using CompillerServices.Backend.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CompillerServices.DependencyInjection
{
    public static class CompillerServiceCollectionExtensions
    {
        public static IServiceCollection AddCompillers(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.TryAddTransient<ISourceWriterFactory, CppFileWriterFactory>();
            serviceCollection.TryAddTransient<IBackendCompiller, BackendCompiller>();
            serviceCollection.TryAddTransient<IProjectFileManager, ProjectFileManager>();

            return serviceCollection;
        }
    }
}