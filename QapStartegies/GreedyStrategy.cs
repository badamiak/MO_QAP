using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MO_QAP.QapStrategies
{
    public class GreedyStrategy : IQapStrategy
    {
        public QapResult<T> SearchBest<T>(Func<IEnumerable<T>, float> scoringStrategy, IEnumerable<T> startingPermutation, CancelationToken cancelationToken)
        {
            var score = scoringStrategy.Invoke(startingPermutation);
            var steps = 0L;
            var seenSolutions = 0L;
            var best = new QapResult<T>(startingPermutation,score, steps);
            var watch = new Stopwatch();
            watch.Start();

            while(!cancelationToken.IsCancelationPending())
            {
                var hasNewBest = false;
                foreach(var permutation in QapMath.Opt2(best.Solution))
                {
                    steps++;
                    seenSolutions++;
                    score = scoringStrategy.Invoke(permutation);

                    if(score < best.Score)
                    {
                        best = new QapResult<T>(permutation, score, steps, seenSolutions);
                        hasNewBest = true;
                        break;
                    }
                }
                if(!hasNewBest) break;
            }
            watch.Stop();

            best.TotalSolutionsSeen = seenSolutions;
            best.FoundIn = watch.Elapsed;
            return best;
        }
    }
}