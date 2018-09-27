using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace MO_QAP
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine($"Reading data from {args[0]}");
            
            var data = ReadData(args[0]);

            var permutation = Math.Permutate(Math.Range(0,5));

            System.Console.WriteLine(string.Join(",",permutation));

            var opt2n = Math.Opt2(permutation);

            var test = new[] {11,15,26,7,4,13,12,2,6,18,9,5,1,21,8,14,3,20,19,10,17,25,16,24,22,23};
            System.Console.WriteLine(Score(data, test));
        }

        static float Score<T>(DataMatrices data, IEnumerable<T> permutation)
        {
            var permutationArray = permutation.Cast<int>().Select(x=>x-1).ToArray();
            var score = 0f;
            var maxIndex = permutation.Count()-1;
            
            foreach(var i in MO_QAP.Math.Range(0,maxIndex))
                foreach(var j in MO_QAP.Math.Range(0,maxIndex))
                {
                    score += data.MatrixA[i,j] * data.MatrixB[permutationArray[i],permutationArray[j]];
                }
            return score;
        }

        static DataMatrices ReadData(string path)
        {

            int[,] ParseMatrix(int size, IEnumerable<string> rawData)
            {
                int[,] matrix = new int[size,size];
                
                int x = 0;
                foreach(var line in rawData)
                {
                    var values = line.Split(' ');
                    int y = 0;
                    foreach(var value in values.Where(v=>!string.IsNullOrEmpty(v)))
                    {
                        matrix[x,y] = int.Parse(value);
                        y++;
                    }
                    x++;
                }
                return matrix;
            }

            var data = File.ReadAllLines(path).Where(x=>!string.IsNullOrEmpty(x)).ToArray();
            var result = new DataMatrices(int.Parse(data[0].Trim()));

            data = data.Skip(1).ToArray();

            var rawMatrixA = data.Take(result.MatricesSize);
            var rawMatrixb = data.Skip(result.MatricesSize);

            result.MatrixA = ParseMatrix(result.MatricesSize, rawMatrixA);
            result.MatrixB = ParseMatrix(result.MatricesSize, rawMatrixb);

            return result;
        }
    }
}
