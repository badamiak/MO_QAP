using System;
using MO_QAP;
using System.Collections.Generic;

namespace MO_QAP.QapStrategies
{
    public class RandomStrategy : IQapStrategy
    {
        public QapResult<T> SearchBest<T>(Func<IEnumerable<T>, float> scoringStrategy, IEnumerable<T> startingPermutation, bool cancelationToken = false)
        {
            long steps = 0;
            var best = new QapResult<T>(startingPermutation, scoringStrategy.Invoke(startingPermutation), steps);
            var last = startingPermutation;
            while(!cancelationToken)
            {
                steps++;
                last = Math.Permutate(last);
                var score = scoringStrategy.Invoke(last);
                if(score > best.Score)
                {
                    best = new QapResult<T>(last, score, steps);
                }
            }

            return best;
        }
    }
}