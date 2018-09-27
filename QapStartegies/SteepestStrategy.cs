using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MO_QAP.QapStrategies
{
    public class SteepestStrategy : IQapStrategy
    {
        public QapResult<T> SearchBest<T>(Func<IEnumerable<T>, float> scoringStrategy, IEnumerable<T> startingPermutation, CancelationToken cancelationToken)
        {
            var score = scoringStrategy.Invoke(startingPermutation);
            var steps = 0;
            var best = new QapResult<T>(startingPermutation,score, steps);
            var seenSolutions = 0;
            var watch = new Stopwatch();
            watch.Start();

            while(!cancelationToken.IsCancelationPending())
            {
                hasNewBest = false;
                foreach(var permutation in QapMath.Opt2(best.Solution))
                {
                    steps++;
                    seenSolutions++;
                    score = scoringStrategy.Invoke(permutation);

                    if(score < best.Score)
                    {
                        best = new QapResult<T>(permutation, score, steps,seenSolutions);
                        best.FoundIn = watch.Elapsed;
                    }
                }
            }
            watch.Stop();
            best.TotalSolutionsSeen = seenSolutions;
            return best;
        }
    }
}