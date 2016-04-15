namespace SkyCrab.Connection.PresentationLayer.Messages
{
    
    public abstract class AbstractMessage
    {

        public abstract MessageId Id { get; }

        internal abstract bool Answer { get; }


        internal abstract object Read(MessageConnection connection);

    }
}
