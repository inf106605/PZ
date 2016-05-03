using System;

namespace SkyCrab.Common_classes.Rooms.Rules
{
    public class TimeRule : Rule<UInt32>
    {

        public UInt32 minTime;
        public UInt32 maxTime;
        
        public TimeRule()
        {
        }

        public TimeRule(Rule<UInt32> rule) :
            base(rule)
        {
        }

    }
}
