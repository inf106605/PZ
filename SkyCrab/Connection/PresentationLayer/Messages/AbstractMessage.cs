using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.Utils;

namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public abstract class AbstractMessage
    {

        public abstract MessageId Id { get; }

        internal abstract bool Answer { get; }
        
        internal abstract object Read(MessageConnection connection);
        
        protected delegate void AsyncPost(AnswerCallback callback, object state);

        protected static MessageInfo? SyncPost(AsyncPost asyncPost, int timeout)
        {
            using (AnswerSynchronizer synchronizer = new AnswerSynchronizer())
            {
                asyncPost.Invoke(AnswerSynchronizer.Callback, synchronizer);
                synchronizer.Wait(timeout);
                return synchronizer.Answer;
            }
        }

    }
}
