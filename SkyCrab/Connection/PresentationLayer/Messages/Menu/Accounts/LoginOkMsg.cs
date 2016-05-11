using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Players;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.LOGIN_OK"/></para>
    /// <para>Data type: <see cref="Player"/> (without password)</para>
    /// <para>Possible answers: [none]</para>
    /// </summary>
    public sealed class LoginOkMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LOGIN_OK; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            Player player = PlayerTranscoder.Get.Read(connection);
            return player;
        }

        public static void AsyncPost(Int16 id, MessageConnection connection, Player player)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    PlayerTranscoder.Get.Write(connection, writingBlock, player);
            connection.PostAnswerMessage(id, MessageId.LOGIN_OK, messageProcedure);
        }

    }
}
