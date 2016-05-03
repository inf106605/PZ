namespace SkyCrab.Common_classes.Rooms.Rules
{
    public class Rule<T>
    {

        public bool indifferently;
        public T value;

        public Rule()
        {
        }

        public Rule(Rule<T> rule)
        {
            this.indifferently = rule.indifferently;
            this.value = rule.value;
        }

    }
}
