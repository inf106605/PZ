using SkyCrab.Common_classes.Rooms.Rules;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms.Rules
{
    internal sealed class TimeRuleTranscoder : AbstractTranscoder<TimeRule>
    {

        private static readonly TimeRuleTranscoder instance = new TimeRuleTranscoder();
        public static TimeRuleTranscoder Get
        {
            get { return instance; }
        }


        private TimeRuleTranscoder()
        {
        }

        public override TimeRule Read(EncryptedConnection connection)
        {
            Rule<UInt32> basicRule = RuleTranscoder<UInt32>.Get(UInt32Transcoder.Get).Read(connection);
            TimeRule data = new TimeRule(basicRule);
            data.minTime = UInt32Transcoder.Get.Read(connection);
            data.maxTime = UInt32Transcoder.Get.Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, TimeRule data)
        {
            RuleTranscoder<UInt32>.Get(UInt32Transcoder.Get).Write(connection, writingBlock, data);
            UInt32Transcoder.Get.Write(connection, writingBlock, data.minTime);
            UInt32Transcoder.Get.Write(connection, writingBlock, data.maxTime);
        }

    }
}
