using System;
using System.Collections;
using System.Collections.Generic;

namespace Kopiarka.Classes
{
    class DateEnumerable : IEnumerable<DateTime>
    {
        public Period Period { get; private set; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }

        public DateEnumerable(Period period, DateTime from, DateTime to)
        {
            Period = period;
            From = from;
            To = to;
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public static IEnumerable<DateTime> EachMonth(DateTime from, DateTime to)
        {
            for (var month = from.Date; month.Date <= to.Date; month = month.AddMonths(1))
                yield return month;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            return Period == Period.Daily ? EachDay(From, To).GetEnumerator() : EachMonth(From, To).GetEnumerator();
        }
    }
}
