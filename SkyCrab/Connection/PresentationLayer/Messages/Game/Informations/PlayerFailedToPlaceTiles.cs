using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.PLAYER_FAILED"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: <see cref="WordOnBoard"/></para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class PlayerFailedToPlaceTilesMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_FAILED; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            WordOnBoard wordOnBoard = WordOnBoardTranscoder.Get.Read(connection);
            return wordOnBoard;
        }

        public static void AsyncPost(MessageConnection connection, WordOnBoard wordOnBoard)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
                    WordOnBoardTranscoder.Get.Write(connection, writingBlock, wordOnBoard);
            connection.PostNewMessage(MessageId.PLAYER_FAILED, messageProc);
        }
    }
}
