using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Initialize;
using LanguageServerProtocol.Handlers.Initialized;
using LanguageServerProtocol.IPC.Client;
using Microsoft.Extensions.Options;
using PluginServer.Settings;

namespace PluginServer.Handlers
{
    public class InitializedHandler : DefaultInitilalizedHandler
    {
        private readonly ICapabilityRegisterer _capabilityRegisterer;
        private readonly IOptions<LanguageOptions> _languageOptions;

        public InitializedHandler(IOptions<LanguageOptions> languageOptions, ICapabilityRegisterer capabilityRegisterer)
        {
            _languageOptions = languageOptions;
            _capabilityRegisterer = capabilityRegisterer;
        }

        public override Task Handle()
        {
            string[] languages = _languageOptions.Value.SupportedLanguages.ToArray();

            IList<Registration> registrations = new List<Registration>
            {
                Registration.GetDidOpenRegistration(languages),
                Registration.GetDidChangeRegistration(TextDocumentSyncKind.Incremental, languages),
                Registration.GetWillSaveRegistration(languages),
                Registration.GetWillSaveWaitUntilRegistration(languages),
                Registration.GetDidSaveRegistration(true, languages),
                Registration.GetDidCloseRegistration(languages),
                Registration.GetCompletionRegistration(null, true, languages)
            };

            _capabilityRegisterer.RegisterCapability(registrations);

            return Task.CompletedTask;
        }
    }
}