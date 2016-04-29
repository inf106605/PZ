using SkyCrab.Common_classes.Rooms.Rules;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms.Rules;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.FIND_ROOMS"/></para>
    /// <para>Data type: <see cref="RuleSet"/> (rule filter)</para>
    /// <para>Passible answers: <see cref="RoomListMsg"/></para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class FindRoomsMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.FIND_ROOMS; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            RuleSet ruleFilter = RuleSetTranscoder.Get.Read(connection);
            return ruleFilter;
        }

        public static MessageInfo? SyncPostFindRooms(MessageConnection connection, RuleSet ruleFilter, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostFindRooms(connection, ruleFilter, callback, state), timeout);
        }

        public static void AsyncPostFindRooms(MessageConnection connection, RuleSet ruleFilter, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
                RuleSetTranscoder.Get.Write(connection, writingBlock, ruleFilter);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.FIND_ROOMS, messageProc);
        }

    }
}
