namespace Easy.Common.MQ
{
    public class MessageInfo<T>
    {
        public string MessageId { get; set; }

        public T Value { get; set; }
    }
}
