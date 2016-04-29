using SkyCrab.Common_classes.Rooms.Rules;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

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
            data.maxRoundTIme = TimeRuleTranscoder.Get.Read(connection);
            data.fivesFirst = RuleTranscoder<bool>.Get(BoolTranscoder.Get).Read(connection);
            data.restrictedExchange = RuleTranscoder<bool>.Get(BoolTranscoder.Get).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, RuleSet data)
        {
            TimeRuleTranscoder.Get.Write(connection, writingBlock, data.maxRoundTIme);
            RuleTranscoder<bool>.Get(BoolTranscoder.Get).Write(connection, writingBlock, data.fivesFirst);
            RuleTranscoder<bool>.Get(BoolTranscoder.Get).Write(connection, writingBlock, data.restrictedExchange);
        }

    }
}
