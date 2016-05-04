using System;

namespace SkyCrab.Common_classes.Rooms.Rules
{
    public class Rule<T> where T : IComparable
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

        public bool Math(Rule<T> filter)
        {
            if (indifferently)
                return true;
            else
                return value.CompareTo(filter.value) == 0;
        }

    }
}
