using SkyCrab.Common_classes.Rooms.Rules;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms.Rules
{
    internal sealed class RangeRuleTranscoder<T> : AbstractTranscoder<RangeRule<T>>
    {

        private static RangeRuleTranscoder<T> instance;
        public static RangeRuleTranscoder<T> Get(AbstractTranscoder<T> tTrancoder)
        {
            if (instance == null)
                instance = new RangeRuleTranscoder<T>(tTrancoder);
            return instance;
        }

        private readonly AbstractTranscoder<T> tTrancoder;


        private RangeRuleTranscoder(AbstractTranscoder<T> tTrancoder)
        {
            this.tTrancoder = tTrancoder;
        }

        public override RangeRule<T> Read(EncryptedConnection connection)
        {
            Rule<T> basicRule = RuleTranscoder<T>.Get(tTrancoder).Read(connection);
            RangeRule<T> data = new RangeRule<T>(basicRule);
            data.min = tTrancoder.Read(connection);
            data.max = tTrancoder.Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, RangeRule<T> data)
        {
            RuleTranscoder<T>.Get(tTrancoder).Write(connection, writingBlock, data);
            tTrancoder.Write(connection, writingBlock, data.min);
            tTrancoder.Write(connection, writingBlock, data.max);
        }

    }
}
