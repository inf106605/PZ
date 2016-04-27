using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.LOGIN_OK"/></para>
    /// <para>Data type: <see cref="Player"/> (without password)</para>
    /// <para>Passible answers: [none]</para>
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
            Player player = connection.SyncReadData(PlayerTranscoder.Get);
            return player;
        }

        public static void AsyncPostLoginOk(MessageConnection connection, Player player)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.AsyncWriteData(UInt32Transcoder.Get, writingBlock, player.Id);
                connection.AsyncWriteData(PlayerProfileTranscoder.Get, writingBlock, player.Profile);
            };
            connection.PostMessage(MessageId.LOGIN_OK, messageProcedure);
        }

    }
}
