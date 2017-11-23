using System;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.Messages;
using Newtonsoft.Json.Linq;

namespace LanguageServerProtocol.IPC.Window
{
    public class MessageSender : IMessageSender
    {
        private readonly IInput _input;
        private readonly IOutput _output;

        public MessageSender(IInput input, IOutput output)
        {
            _input = input;
            _output = output;
        }

        public async Task ShowMessage(ShowMessageParams showMessageParams,
            CancellationToken cancellationToken = default)
        {
            Request request = new Request(MessageId.Empty, "window/showMessage", showMessageParams);
            await _output.WriteAsync(JToken.FromObject(request), cancellationToken);
        }

        public async Task<MessageActionItem> ShowMessageRequest(ShowMessageRequestParams showMessageRequestParams,
            CancellationToken cancellationToken = default)
        {
            IRequest request = new Request(new MessageId(Guid.NewGuid().ToString()), "windows/showMessageRequest",
                showMessageRequestParams);

            await _output.WriteAsync(JToken.FromObject(request), cancellationToken);
            JToken responseToken = await _input.ReadAsync(cancellationToken);

            Response<MessageActionItem> response = responseToken.ToObject<Response<MessageActionItem>>();
            return response.Result;
        }
    }
}