using System;
using CompillerServices.Backend;
using CompillerServices.Backend.EntryPoint;
using CompillerServices.Backend.TranslatorFactories;
using CompillerServices.Backend.Writers;
using CompillerServices.Frontend;
using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;
using CompillerServices.Output;
using CompillerServices.ProjectFile;
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

            serviceCollection.TryAddTransient<IFileLoader, FileLoader>();

            serviceCollection.TryAddTransient<ILexerFactory, LexerFactory>();
            serviceCollection.TryAddTransient<IParserFactory, ParserFactory>();
            serviceCollection.TryAddTransient<ITranslatorFactory, CppTranslatorFactory>();

            serviceCollection.TryAddTransient<IBackendCompiller, BackendCompiller>();
            serviceCollection.TryAddTransient<IProjectFileManager, ProjectFileManager>();
            serviceCollection.TryAddTransient<IEntryPointWriter, EntryPointWriter>();
            serviceCollection.TryAddTransient<IOutputWriter, ConsoleOutputWriter>();

            serviceCollection.TryAddTransient<IFrontendCompiller, FrontendCompiller>();
            serviceCollection.TryAddTransient<INameTableContainer, NameTableContainer>();

            return serviceCollection;
        }
    }
}