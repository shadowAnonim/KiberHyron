namespace KiberHyron.Data
{
    public class MessagesData : JsonData
    {
        public List<ReactableMessage> messages { get; set; } = new List<ReactableMessage>();
    }
}
