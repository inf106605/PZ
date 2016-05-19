using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Commands
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.PLACE_TILES"/></para>
    /// <para>Data type: <see cref="TilesToPlace"/></para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_GAME2"/>, <see cref="ErrorCode.NOT_YOUR_TURN"/>, <see cref="ErrorCode.INCORRECT_MOVE2"/>, <see cref="ErrorCode.TOO_LESS_TILES"/>, <see cref="ErrorCode.FIVES_FIRST_VIOLATION"/>, <see cref="ErrorCode.LETTERS_NOT_FROM_RACK"/>, <see cref="ErrorCode.LETTERS_NOT_MATH"/>, <see cref="ErrorCode.WORD_NOT_IN_LINE"/>, <see cref="ErrorCode.NOT_IN_STARTING_SQUARE"/>, <see cref="ErrorCode.NOT_ADJANCENT"/>, <see cref="ErrorCode.NOT_CONTINUOUS"/>, <see cref="ErrorCode.INCORECT_WORD"/></para>
    /// </summary>
    public sealed class PlaceTilesMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLACE_TILES; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            TilesToPlace tilesToPlace = TilesToPlaceTranscoder.Get.Read(connection);
            return tilesToPlace;
        }

        public static MessageInfo? SyncPost(MessageConnection connection, TilesToPlace tilesToPlace, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, tilesToPlace, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, TilesToPlace tilesToPlace, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    TilesToPlaceTranscoder.Get.Write(connection, writingBlock, tilesToPlace);
            connection.PostNewMessage(MessageId.PLACE_TILES, messageProcedure, callback, state);
        }

        public static void AsyncPost(MessageConnection connection, TilesToPlace tilesToPlace)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    TilesToPlaceTranscoder.Get.Write(connection, writingBlock, tilesToPlace);
            connection.PostNewMessage(MessageId.PLACE_TILES, messageProcedure);
        }
    }
}
