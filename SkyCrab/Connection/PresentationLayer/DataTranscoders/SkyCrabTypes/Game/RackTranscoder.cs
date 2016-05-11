using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class RackTranscoder : AbstractTranscoder<Rack>
    {

        private static readonly RackTranscoder instance = new RackTranscoder();
        public static RackTranscoder Get
        {
            get { return instance; }
        }


        private RackTranscoder()
        {
        }

        public override Rack Read(EncryptedConnection connection)
        {
            List<TileOnRack> tiles = ListTranscoder<TileOnRack>.Get(TileOnRackTranscoder.Get).Read(connection);
            Rack data = new Rack(tiles);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, Rack data)
        {
            ListTranscoder<TileOnRack>.Get(TileOnRackTranscoder.Get).Write(connection, writingBlock, new List<TileOnRack>(data.Tiles));
        }

    }
}
