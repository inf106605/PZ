using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Games
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.GAME_LOG"/></para>
    /// <para>Data type: <see cref="string"/> (game log)</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class GameLogMsg : AbstractMessage
    {
        
        public override MessageId Id
        {
            get { return MessageId.GAME_LOG; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            string gameLog = StringTranscoder.Get.Read(connection);
            return gameLog;
        }

        public static void AsyncPost(Int16 id, MessageConnection connection, string gameLog)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
                StringTranscoder.Get.Write(connection, writingBlock, gameLog);
            connection.PostAnswerMessage(id, MessageId.GAME_LOG, messageProc);
        }

    }
}
