using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Initialize;
using LanguageServerProtocol.Handlers.Initialized;
using LanguageServerProtocol.IPC.Client;

namespace PluginServer.Handlers
{
    public class InitializedHandler : DefaultInitilalizedHandler
    {
        private readonly ICapabilityRegisterer _capabilityRegisterer;

        public InitializedHandler(ICapabilityRegisterer capabilityRegisterer)
        {
            _capabilityRegisterer = capabilityRegisterer;
        }

        public override Task Handle()
        {
            const string language = "slang";

            IList<Registration> registrations = new List<Registration>
            {
                Registration.GetDidOpenRegistration(language),
                Registration.GetDidChangeRegistration(TextDocumentSyncKind.Incremental, language),
                Registration.GetWillSaveRegistration(language),
                Registration.GetWillSaveWaitUntilRegistration(language),
                Registration.GetDidSaveRegistration(true, language),
                Registration.GetDidCloseRegistration(language),
                Registration.GetCompletionRegistration(null, true, language)
            };

            _capabilityRegisterer.RegisterCapability(registrations);

            return Task.CompletedTask;
        }
    }
}