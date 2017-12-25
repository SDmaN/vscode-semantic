using System;
using CompillerServices.Backend;
using CompillerServices.Backend.EntryPoint;
using CompillerServices.Backend.ProjectFile;
using CompillerServices.Backend.Writers;
using CompillerServices.Output;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlangGrammar.Factories;

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

            serviceCollection.TryAddTransient<ILexerFactory, LexerFactory>();
            serviceCollection.TryAddTransient<IParserFactory, ParserFactory>();
            serviceCollection.TryAddTransient<ISourceWriterFactory, CppFileWriterFactory>();
            serviceCollection.TryAddTransient<IBackendCompiller, BackendCompiller>();
            serviceCollection.TryAddTransient<IProjectFileManager, ProjectFileManager>();
            serviceCollection.TryAddTransient<IEntryPointWriter, EntryPointWriter>();
            serviceCollection.TryAddTransient<IOutputWriter, ConsoleOutputWriter>();

            return serviceCollection;
        }
    }
}