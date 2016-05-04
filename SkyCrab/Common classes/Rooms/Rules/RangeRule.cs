using System;

namespace SkyCrab.Common_classes.Rooms.Rules
{
    public class RangeRule<T> : Rule<T>
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

    }
}
