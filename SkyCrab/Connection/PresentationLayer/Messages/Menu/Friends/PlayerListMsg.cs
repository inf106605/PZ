using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Players;
using System;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.PLAYER_LIST"/></para>
    /// <para>Data type: <see cref="List{T}"/> of <see cref="Player"/>s</para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class PlayerListMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_LIST; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            List<Player> players = ListTranscoder<Player>.Get(PlayerTranscoder.Get).Read(connection);
            return players;
        }

        public static void AsyncPost(UInt16 id, MessageConnection connection, List<Player> players)
        {
            MessageConnection.MessageProcedure messsageProc = (writingBlock) =>
            {
                ListTranscoder<Player>.Get(PlayerTranscoder.Get).Write(connection, writingBlock, players);
            };
            connection.PostAnswerMessage(id, MessageId.PLAYER_LIST, messsageProc);
        }

    }
}
