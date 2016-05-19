using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.REORDER_RACK_TILES"/></para>
    /// <para>Data type: <see cref="Rack"/></para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_GAME"/>, <see cref="ErrorCode.INCORRECT_MOVE"/></para>
    /// </summary>
    public sealed class ReorderRackTilesMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.REORDER_RACK_TILES; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            Rack rack = RackTranscoder.Get.Read(connection);
            return rack;
        }

        public static void AsyncPost(MessageConnection connection, Rack rack)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    RackTranscoder.Get.Write(connection, writingBlock, rack);
            connection.PostNewMessage(MessageId.REORDER_RACK_TILES, messageProcedure);
        }
    }
}
