using System;

namespace SkyCrab.Common_classes.Rooms.Rules
{
    public class RangeRule<T> : Rule<T> where T : IComparable
    {

        public T min;
        public T max;
        
        public RangeRule()
        {
        }

        public RangeRule(Rule<T> rule) :
            base(rule)
        {
        }

        public bool Math(RangeRule<T> filter)
        {
            if (indifferently)
                return true;
            if (value.CompareTo(filter.min) < 0)
                return false;
            if (filter.max.CompareTo(default(T)) != 0 && value.CompareTo(filter.max) > 0)
                return false;
            return true;
        }

    }
}
