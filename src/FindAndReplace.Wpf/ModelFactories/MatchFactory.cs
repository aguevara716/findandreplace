using System;
using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class MatchFactory
    {
        public static Match GetMatch(int seed)
        {
            var generator = new Random();
            var m = new Match
            {
                Index = generator.Next(seed),
                Length = generator.Next(seed)
            };
            return m;
        }

        public static IEnumerable<Match> GetMatches(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetMatch(index + 1);
        }
    }
}
