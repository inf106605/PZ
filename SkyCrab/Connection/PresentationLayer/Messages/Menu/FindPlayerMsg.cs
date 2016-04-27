using SkyCrab.Common_classes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.FIND_PLAYER"/></para>
    /// <para>Data type: <see cref="string"/> (search phraze)</para>
    /// <para>Passible answers: <see cref="PlayerListMsg"/></para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class FindPlayerMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.FIND_PLAYER; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            string searchPhrase = connection.SyncReadData(LimitedStringTranscoder.Get(LengthLimit.SearchPhraze));
            return searchPhrase;
        }

        public static MessageConnection.MessageInfo? SyncPostGetFriends(MessageConnection connection, string searchPhrase, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostFindPlayer(connection, searchPhrase, callback, state), timeout);
        }

        public static void AsyncPostFindPlayer(MessageConnection connection, string searchPhrase, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
                connection.AsyncWriteData(LimitedStringTranscoder.Get(LengthLimit.SearchPhraze), writingBlock, searchPhrase);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.FIND_PLAYER, messageProc);
        }

    }
}
