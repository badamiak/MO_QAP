using System;
using System.Collections.Generic;

namespace MO_QAP.QapStrategies
{
    public class SteepestStrategy : IQapStrategy
    {
        public QapResult<T> SearchBest<T>(Func<IEnumerable<T>, float> scoringStrategy, IEnumerable<T> startingPermutation, CancelationToken cancelationToken)
        {
            var score = scoringStrategy.Invoke(startingPermutation);
            var steps = 0;
            var best = new QapResult<T>(startingPermutation,score, steps);

            while(!cancelationToken.IsCancelationPending())
            {
                foreach(var permutation in QapMath.Opt2(best.Solution))
                {
                    steps++;
                    score = scoringStrategy.Invoke(permutation);

                    if(score < best.Score)
                    {
                        best = new QapResult<T>(permutation, score, steps);
                    }
                }
            }
            return best;
        }
    }
}