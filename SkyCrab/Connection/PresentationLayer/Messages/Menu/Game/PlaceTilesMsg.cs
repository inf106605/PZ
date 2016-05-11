using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.PLACE_TILES"/></para>
    /// <para>Data type: <see cref="TilesToPlace"/></para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_GAME2"/>, <see cref="ErrorCode.NOT_YOUR_TURN"/>, <see cref="ErrorCode.INCORRECT_MOVE2"/></para>
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

        public static void AsyncPost(MessageConnection connection, TilesToPlace tilesToPlace)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    TilesToPlaceTranscoder.Get.Write(connection, writingBlock, tilesToPlace);
            connection.PostNewMessage(MessageId.PLACE_TILES, messageProcedure);
        }
    }
}
