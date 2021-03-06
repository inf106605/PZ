﻿using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class LostTilesTranscoder : AbstractTranscoder<LostLetters>
    {

        private static readonly LostTilesTranscoder instance = new LostTilesTranscoder();
        public static LostTilesTranscoder Get
        {
            get { return instance; }
        }


        private LostTilesTranscoder()
        {
        }

        public override LostLetters Read(EncryptedConnection connection)
        {
            LostLetters data = new LostLetters();
            data.playerId = UInt32Transcoder.Get.Read(connection);
            data.backToPouch = BoolTranscoder.Get.Read(connection);
            data.letters = ListTranscoder<LetterWithNumber>.Get(LetterWithNumberTranscoder.Get).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, LostLetters data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.playerId);
            BoolTranscoder.Get.Write(connection, writingBlock, data.backToPouch);
            ListTranscoder<LetterWithNumber>.Get(LetterWithNumberTranscoder.Get).Write(connection, writingBlock, data.letters);
        }

    }
}
