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
            await Broadcast(
                $"ServiceUrl: {message.ServiceUrl} \n ConversationId: {message.Conversation.Id} \n BotAccountId: {message.Recipient.Id} \n BotAccountName: {message.Recipient.Name}",
                "https://facebook.botframework.com/", 
                "1068190953300282-283726938688635", 
                "283726938688635", 
                "gitbot_84AlOPcgTdB" );
            context.Wait(MessageReceivedAsync);
        }

        public async Task Broadcast(string text, string serviceUrl, string conversationId, string botAccountId, string botAccountName)
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