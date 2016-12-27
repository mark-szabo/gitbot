using gitbot.Dialogs;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using gitbot.Models;

namespace gitbot.Controllers
{
    public class WebhookController : ApiController
    {
        public CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        
        /// <summary>
        /// POST: api/Webhook
        /// Receive a webhook and send a message to the user
        /// </summary>
        public async Task<HttpResponseMessage> Post(string id)
        {
            // Get request body.
            HttpContent requestContent = Request.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("conversations");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();            

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<Conversation>("gitbot", id);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            
            if (retrievedResult.Result == null) throw new System.Exception("The conversation could not be retrieved.");

            string text = jsonContent;

            var rootDialog = new RootDialog();
            await rootDialog.SendTo(
                ((Conversation)retrievedResult.Result).ServiceUrl,
                ((Conversation)retrievedResult.Result).ConversationId,
                ((Conversation)retrievedResult.Result).BotAccountId,
                ((Conversation)retrievedResult.Result).BotAccountName,
                text);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
