using System;
using CompillerServices.Backend;
using CompillerServices.Backend.EntryPoint;
using CompillerServices.Backend.Translators;
using CompillerServices.Cpp;
using CompillerServices.Frontend;
using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;
using CompillerServices.Logging;
using CompillerServices.ProjectFile;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
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

            AddLogger(serviceCollection);
            AddLocalization(serviceCollection);

            serviceCollection.TryAddTransient<IFileLoader, FileLoader>();

            serviceCollection.TryAddTransient<ILexerFactory, LexerFactory>();
            serviceCollection.TryAddTransient<IParserFactory, ParserFactory>();
            serviceCollection.TryAddTransient<ITranslatorFactory, CppTranslatorFactory>();

            serviceCollection.TryAddTransient<IBackendCompiller, BackendCompiller>();
            serviceCollection.TryAddTransient<IProjectFileManager, ProjectFileManager>();
            serviceCollection.TryAddTransient<IEntryPointWriter, EntryPointWriter>();

            serviceCollection.TryAddTransient<IFrontendCompiller, FrontendCompiller>();
            serviceCollection.TryAddTransient<INameTableContainer, NameTableContainer>();
            serviceCollection.TryAddTransient<IStepVisitorFactory, StepVisitorFactory>();

            serviceCollection.TryAddTransient<ICppCompillerAdapter, GccAdapter>();

            return serviceCollection;
        }

        private static void AddLogger(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(logBuilder => logBuilder.AddProvider(new CompillerLoggerProvider()));
        }

        private static void AddLocalization(IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions();
            serviceCollection.Configure<LocalizationOptions>(options => options.ResourcesPath = "Resources");
            serviceCollection.AddLocalization();
        }
    }
}