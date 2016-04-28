﻿using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class PlayerTranscoder : ITranscoder<Player>
    {

        private static readonly PlayerTranscoder instance = new PlayerTranscoder();
        public static PlayerTranscoder Get
        {
            get { return instance; }
        }


        private PlayerTranscoder()
        {
        }

        public Player Read(DataConnection connection)
        {
            uint id = UInt32Transcoder.Get.Read(connection);
            bool hasProfile = BoolTranscoder.Get.Read(connection);
            Player player;
            if (hasProfile)
            {
                PlayerProfile playerProfile = PlayerProfileTranscoder.Get.Read(connection);
                player = new Player(id, playerProfile);
            }
            else
            {
                bool isGuest = BoolTranscoder.Get.Read(connection);
                string nick = StringTranscoder.Get.Read(connection);
                player = new Player(id, isGuest, nick);
            }
            return player;
        }

        public void Write(DataConnection connection, object writingBlock, Player data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.Id);
            BoolTranscoder.Get.Write(connection, writingBlock, data.Profile != null);
            if (data.Profile == null)
            {
                BoolTranscoder.Get.Write(connection, writingBlock, data.IsGuest);
                StringTranscoder.Get.Write(connection, writingBlock, data.Nick);
            }
            else
            {
                PlayerProfileTranscoder.Get.Write(connection, writingBlock, data.Profile);
            }
        }

    }
}
