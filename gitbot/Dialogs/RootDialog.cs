using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            await Broadcast(new Uri(message.ServiceUrl), message.Conversation);
            context.Wait(MessageReceivedAsync);
        }

        public async Task Broadcast(Uri serviceUrl, ConversationAccount conversation)
        {
            /*var connector = new ConnectorClient(serviceUrl);
            IMessageActivity newMessage = Activity.CreateMessageActivity();
            newMessage.Type = ActivityTypes.Message;
            newMessage.From = botAccount;
            newMessage.Conversation = conversation;
            newMessage.Recipient = userAccount;
            newMessage.Text = "Yo yo yo!";
            await connector.Conversations.SendToConversationAsync((Activity)newMessage);*/
        }
    }
}