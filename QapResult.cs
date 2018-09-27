using System.Collections.Generic;
using System;

namespace MO_QAP
{
    public class QapResult<T>
    {
        public QapResult(IEnumerable<T> solution, float score, long steps, long seenSolution = 1)
        {
            this.Solution = solution;
            this.Score = score;
            this.Steps = steps;
            this.seenSolution = seenSolution;
            this.FoundIn = TimeSpan.FromSeconds(-1);
        }

        public IEnumerable<T> Solution {get;}
        public float Score {get;}
        public long Steps {get;}
        public long seenSolution{get;}
        public long totalSolutionsSeen{get;set;} = 0;
        public TimeSpan FoundIn {get; set;}
    }
}