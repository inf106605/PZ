using SkyCrab.Common_classes.Rooms.Rules;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms.Rules
{
    internal sealed class RuleSetTranscoder : AbstractTranscoder<RuleSet>
    {

        private static readonly RuleSetTranscoder instance = new RuleSetTranscoder();
        public static RuleSetTranscoder Get
        {
            get { return instance; }
        }


        private RuleSetTranscoder()
        {
        }

        public override RuleSet Read(EncryptedConnection connection)
        {
            RuleSet data = new RuleSet();
            data.maxRoundTime = RangeRuleTranscoder<UInt32>.Get(UInt32Transcoder.Get).Read(connection);
            data.maxPlayerCount = RangeRuleTranscoder<byte>.Get(UInt8Transcoder.Get).Read(connection);
            data.fivesFirst = RuleTranscoder<bool>.Get(BoolTranscoder.Get).Read(connection);
            data.restrictedExchange = RuleTranscoder<bool>.Get(BoolTranscoder.Get).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, RuleSet data)
        {
            RangeRuleTranscoder<UInt32>.Get(UInt32Transcoder.Get).Write(connection, writingBlock, data.maxRoundTime);
            RangeRuleTranscoder<byte>.Get(UInt8Transcoder.Get).Write(connection, writingBlock, data.maxPlayerCount);
            RuleTranscoder<bool>.Get(BoolTranscoder.Get).Write(connection, writingBlock, data.fivesFirst);
            RuleTranscoder<bool>.Get(BoolTranscoder.Get).Write(connection, writingBlock, data.restrictedExchange);
        }

    }
}
