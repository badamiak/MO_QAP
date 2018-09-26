using System.Collections.Generic;

namespace MO_QAP
{
    public class QapResult<T>
    {
        public QapResult(IEnumerable<T> solution, float score, long steps)
        {
            this.solution = solution;
            this.Score = score;
            this.Steps = steps;
        }

        public IEnumerable<T> solution {get;}
        public float Score {get;}
        public long Steps {get;}
    }
}