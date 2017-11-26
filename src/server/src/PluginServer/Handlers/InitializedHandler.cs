using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.Initialized;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.IPC.Client;
using Newtonsoft.Json;

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
            _capabilityRegisterer.RegisterCapability(new RegistrationParams
            {
                Registrations = new List<Registration>
                {
                    new Registration
                    {
                        Id = Guid.NewGuid().ToString(),
                        Method = "textDocument/didOpen",
                        RegisterOptions = new TextDocumentRegistrationOptions
                        {
                            DocumentSelector = new List<DocumentFilter>
                            {
                                new DocumentFilter
                                {
                                    Language = "slang"
                                }
                            }
                        }
                    }
                }
            });
            
            return Task.CompletedTask;
        }
    }

    public class TextDocumentRegistrationOptions
    {
        [JsonProperty("documentSelector")]
        public IEnumerable<DocumentFilter> DocumentSelector { get; set; }
    }
}