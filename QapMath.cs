using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace MO_QAP
{
    public static class QapMath
    {
        ///Use for generating increasing value ranges
        ///<param name="to">inclusive</param>
        ///<param name="step">Default = 1</param>
        public static IEnumerable<int> Range(int from, int to, int step=1)
        {
            var range = new List<int>();
            while(from <= to)
            {
                range.Add(from);
                from+=step;
            }
            return range;
        }

        public static IEnumerable<T> Permutate<T>(IEnumerable<T> values)
        {
            var indexesAvailability = new Dictionary<int,bool>();
            var distinctValues = values.Distinct().ToArray();

            foreach(var index in Range(0,distinctValues.Count()-1))
            {
                indexesAvailability.Add(index, true);
            }

            var rand = new Random();
            Func<IEnumerable<KeyValuePair<int,bool>>> available = () => indexesAvailability.Where(x=>x.Value);

            var result = new List<T>();

            while(available().Count() > 0)
            {
                var r = rand.Next(0,available().Count());
                var key = available().ToList()[r].Key;
                indexesAvailability[key]=false;
                result.Add(distinctValues[key]);
            }

            return result;
        }

        public static IEnumerable<IEnumerable<T>> Opt2<T>(IEnumerable<T> inputPermutation)
        {
            IEnumerable<T2> ChangePositions<T2>(IEnumerable<T2> input, int thisPosition, int withThatPosition)
            {
                var inputAsArray = input.ToArray();
                var firstElement = inputAsArray[thisPosition];
                
                inputAsArray[thisPosition] = inputAsArray[withThatPosition];
                inputAsArray[withThatPosition] = firstElement;

                return inputAsArray;
            }

            var possibleChangesCatalogue = new List<Tuple<int,int>>();
            var lastIndex = inputPermutation.Count();

            foreach(var firstIndex in Range(0,lastIndex-1))
            {
                foreach(var secondIndex in Range(firstIndex+1,lastIndex-1))
                {
                    possibleChangesCatalogue.Add(Tuple.Create(firstIndex, secondIndex));
                }
            }

            foreach(var change in possibleChangesCatalogue)
            {
                yield return ChangePositions(inputPermutation, change.Item1, change.Item2);
            }
        }
    }
}