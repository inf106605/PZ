using SkyCrab.Common_classes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.FIND_PLAYERS"/></para>
    /// <para>Data type: <see cref="string"/> (search phraze)</para>
    /// <para>Possible answers: <see cref="PlayerListMsg"/></para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class FindPlayersMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.FIND_PLAYERS; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            string searchPhrase = LimitedStringTranscoder.Get(LengthLimit.SearchPhraze).Read(connection);
            return searchPhrase;
        }

        public static MessageInfo? SyncPost(MessageConnection connection, string searchPhrase, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, searchPhrase, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, string searchPhrase, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
                LimitedStringTranscoder.Get(LengthLimit.SearchPhraze).Write(connection, writingBlock, searchPhrase);
            };
            connection.PostNewMessage(MessageId.FIND_PLAYERS, messageProc, callback, state);
        }

    }
}
