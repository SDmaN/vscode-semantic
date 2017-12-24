using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.Messages;
using Newtonsoft.Json.Linq;

namespace LanguageServerProtocol.IPC.Client
{
    public class CapabilityRegisterer : ICapabilityRegisterer
    {
        private readonly IClientResponseManager _clientResponseManager;
        private readonly IDictionary<string, string> _methodCapabilityId = new ConcurrentDictionary<string, string>();
        private readonly IOutput _output;

        public CapabilityRegisterer(IOutput output, IClientResponseManager clientResponseManager)
        {
            _output = output;
            _clientResponseManager = clientResponseManager;
        }

        public Task<IResponse> RegisterCapability(RegistrationParams registrations)
        {
            MessageId messageId = new MessageId(Guid.NewGuid().ToString());

            TaskCompletionSource<IResponse> taskCompletionSource = new TaskCompletionSource<IResponse>();

            _clientResponseManager.RegisterHandler<object>(messageId,
                response =>
                {
                    if (response.Error == null)
                    {
                        foreach (Registration registration in registrations.Registrations)
                        {
                            _methodCapabilityId.Add(registration.Method, registration.Id);
                        }
                    }

                    taskCompletionSource.SetResult(response);
                });

            IRequest request = new Request(messageId, "client/registerCapability", registrations);

            _output.WriteAsync(JToken.FromObject(request))
                .ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        taskCompletionSource.SetCanceled();
                    }

                    if (task.IsFaulted)
                    {
                        taskCompletionSource.SetException(task.Exception);
                    }
                });

            return taskCompletionSource.Task;
        }
    }
}