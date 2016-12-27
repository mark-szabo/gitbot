using Microsoft.WindowsAzure.Storage.Table;

namespace gitbot.Models
{
    public class Conversation : TableEntity
    {
        public Conversation(string id)
        {
            PartitionKey = "gitbot";
            RowKey = id;
        }

        public Conversation() { }

        public string ServiceUrl { get; set; }
        public string ConversationId { get; set; }
        public string BotAccountId { get; set; }
        public string BotAccountName { get; set; }
    }
}