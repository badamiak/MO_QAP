using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MO_QAP.QapStrategies
{
    public class RandomStrategy : IQapStrategy
    {
        public QapResult<T> SearchBest<T>(Func<IEnumerable<T>, float> scoringStrategy, IEnumerable<T> startingPermutation, CancelationToken cancelationToken)
        {
            long steps = 0;
            var best = new QapResult<T>(startingPermutation, scoringStrategy.Invoke(startingPermutation), steps);
            var last = startingPermutation;
            var watch = new Stopwatch();
            watch.Start();

            while(!cancelationToken.IsCancelationPending())
            {
                steps++;
                last = QapMath.Permutate(last);
                var score = scoringStrategy.Invoke(last);
                if(score < best.Score)
                {
                    best = new QapResult<T>(last, score, steps,steps);
                    best.FoundIn = watch.Elapsed;
                }
            }

            watch.Stop();
            best.totalSolutionsSeen = steps;
            return best;
        }
    }
}