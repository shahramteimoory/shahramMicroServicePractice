namespace ProductService.MessagingBus
{
    public interface IMessageBus
    {
        void SendMessage(BaseMessage baseMessage,string exchange);
    }
}
