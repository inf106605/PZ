using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.PLAYER_EXCHAN_TILES"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: <see cref="byte"/> (tiles count)</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class PlayerExchangedMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_EXCHAN_TILES; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            byte tilesCount = UInt8Transcoder.Get.Read(connection);
            return tilesCount;
        }

        public static void AsyncPost(MessageConnection connection, byte tilesCount)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
                    UInt8Transcoder.Get.Write(connection, writingBlock, tilesCount);
            connection.PostNewMessage(MessageId.PLAYER_EXCHAN_TILES, messageProc);
        }
    }
}
