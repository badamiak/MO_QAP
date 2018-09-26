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

        }

        static float Score<T>(DataMatrices data, IEnumerable<T> permutation)
        {
            var score = 0f;
            var maxIndex = permutation.Count()-1;
            for(int i = 0; i<= maxIndex; i++)
            {
                if(i == maxIndex)
                {
                    score += data.MatrixA[permutation[i],permutation[0]] * data.MatrixB[permutation[i],permutation[0]];
                }
                else
                {
                    score += data.MatrixA[permutation[i],permutation[i+1]] * data.MatrixB[permutation[i],permutation[i+1]];
                }
            }
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
