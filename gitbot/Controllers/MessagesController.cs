using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using gitbot.Dialogs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;

namespace gitbot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference("conversations");

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

                // Read storage
                TableQuery<Models.Conversation> query = new TableQuery<Models.Conversation>()
                    .Where(TableQuery.GenerateFilterCondition("ConversationId", QueryComparisons.Equal, activity.Conversation.Id))
                    .Take(1);

                string result = null;
                foreach (Models.Conversation conversation in table.ExecuteQuery(query))
                {
                    result = conversation.RowKey;
                }

                if (result == null)
                {
                    string id = Guid.NewGuid().ToString();
                    string url = $"http://gitbot.azurewebsites.net/api/webhook/{id}";

                    // Create a new conversation entity.
                    Models.Conversation conversation = new Models.Conversation(id);
                    conversation.ServiceUrl = activity.ServiceUrl;
                    conversation.ConversationId = activity.Conversation.Id;
                    conversation.BotAccountId = activity.Recipient.Id;
                    conversation.BotAccountName = activity.Recipient.Name;

                    // Create the TableOperation object that inserts the conversation entity.
                    TableOperation insertOperation = TableOperation.Insert(conversation);

                    // Execute the insert operation.
                    table.Execute(insertOperation);

                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var replyMessage = activity.CreateReply($"Hi, I'm gitbot! \n If you want me to notify you about activities in your git repositories, add this url to all your webhooks! \n {url}", "en");
                    await connector.Conversations.ReplyToActivityAsync(replyMessage);
                }
                else await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}