using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.PASS"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_GAME4"/>, <see cref="ErrorCode.NOT_YOUR_TURN3"/></para>
    /// </summary>
    public sealed class PassMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PASS; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static void AsyncPost(MessageConnection connection)
        {
            connection.PostNewMessage(MessageId.PASS, null);
        }
    }
}
