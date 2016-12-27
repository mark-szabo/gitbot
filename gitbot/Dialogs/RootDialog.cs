using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace gitbot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await SendTo(
                message.ServiceUrl,
                message.Conversation.Id,
                message.Recipient.Id,
                message.Recipient.Name,
                "Sorry, I'm not a very talkative bot... :)");

            context.Wait(MessageReceivedAsync);
        }

        public async Task SendTo(string serviceUrl, string conversationId, string botAccountId, string botAccountName, string text)
        {
            var connector = new ConnectorClient(new Uri(serviceUrl));
            IMessageActivity newMessage = Activity.CreateMessageActivity();
            newMessage.Type = ActivityTypes.Message;
            newMessage.From = new ChannelAccount { Id = botAccountId, Name = botAccountName};
            newMessage.Conversation = new ConversationAccount { Id = conversationId};
            newMessage.Recipient = new ChannelAccount { Id = conversationId, Name = null};
            newMessage.Text = text;
            await connector.Conversations.SendToConversationAsync((Activity)newMessage);
        }
    }
}