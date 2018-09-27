using System;
using MO_QAP;
using System.Collections.Generic;


namespace MO_QAP.QapStrategies
{
    public interface IQapStrategy
    {
        ///<param name="scoringStrategy">Function accepting currently analyzed solution and returning score for that solution</param>
        QapResult<T> SearchBest<T>(Func<IEnumerable<T>, float> scoringStrategy, IEnumerable<T> startingPermutation, CancelationToken cancelationToken);
    }
}