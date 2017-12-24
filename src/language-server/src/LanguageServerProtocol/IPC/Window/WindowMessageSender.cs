using System;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.Messages;
using Newtonsoft.Json.Linq;

namespace LanguageServerProtocol.IPC.Window
{
    public class WindowMessageSender : IWindowMessageSender
    {
        private readonly IClientResponseManager _clientResponseManager;
        private readonly IOutput _output;

        public WindowMessageSender(IClientResponseManager clientResponseManager, IOutput output)
        {
            _clientResponseManager = clientResponseManager;
            _output = output;
        }

        public async Task ShowMessage(ShowMessageParams showMessageParams,
            CancellationToken cancellationToken = default)
        {
            Request request = new Request(MessageId.Empty, "window/showMessage", showMessageParams);
            await _output.WriteAsync(JToken.FromObject(request), cancellationToken);
        }

        public Task<MessageActionItem> ShowMessageRequest(ShowMessageRequestParams showMessageRequestParams,
            CancellationToken cancellationToken = default)
        {
            IRequest request = new Request(new MessageId(Guid.NewGuid().ToString()), "window/showMessageRequest",
                showMessageRequestParams);

            TaskCompletionSource<MessageActionItem>
                taskCompletionSource = new TaskCompletionSource<MessageActionItem>();

            _clientResponseManager.RegisterHandler<MessageActionItem>(request.Id,
                response => taskCompletionSource.SetResult(response.Result));

            _output.WriteAsync(JToken.FromObject(request), cancellationToken).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    taskCompletionSource.SetCanceled();
                }

                if (task.IsFaulted)
                {
                    taskCompletionSource.SetException(task.Exception);
                }
            }, cancellationToken);

            return taskCompletionSource.Task;
        }

        public async Task LogMessage(LogMessageParams logMessageParams, CancellationToken cancellationToken = default)
        {
            Request request = new Request(MessageId.Empty, "window/logMessage", logMessageParams);
            await _output.WriteAsync(JToken.FromObject(request), cancellationToken);
        }
    }
}