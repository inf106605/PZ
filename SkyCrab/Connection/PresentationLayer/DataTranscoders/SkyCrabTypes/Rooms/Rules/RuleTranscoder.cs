using SkyCrab.Common_classes.Rooms.Rules;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms.Rules
{
    internal sealed class RuleTranscoder<T> : AbstractTranscoder<Rule<T>>
    {

        private static RuleTranscoder<T> instance;
        public static RuleTranscoder<T> Get(AbstractTranscoder<T> tTranscoder)
        {
            if (instance == null)
                instance = new RuleTranscoder<T>(tTranscoder);
            return instance;
        }

        private readonly AbstractTranscoder<T> tTranscoder;


        private RuleTranscoder(AbstractTranscoder<T> tTranscoder)
        {
            this.tTranscoder = tTranscoder;
        }
        
        public override Rule<T> Read(EncryptedConnection connection)
        {
            Rule<T> data = new Rule<T>();
            data.indifferently = BoolTranscoder.Get.Read(connection);
            data.value = tTranscoder.Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, Rule<T> data)
        {
            BoolTranscoder.Get.Write(connection, writingBlock, data.indifferently);
            tTranscoder.Write(connection, writingBlock, data.value);
        }

    }
}
