using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TumblOne
{
    public class ParallelUtils
    {
        public static void While(Func<bool> condition, Action body)
        {
            Parallel.ForEach(IterateUntilFalse(condition), ignored => body());
        }

        private static IEnumerable<bool> IterateUntilFalse(Func<bool> condition)
        {
            while (condition()) yield return true;
        }
    }
}
