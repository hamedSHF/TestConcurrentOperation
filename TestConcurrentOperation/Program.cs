using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConcurrentOperation
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            double[,] matrix1 = new double[500,500];
            double[,] matrix2 = new double[500,100];
            initialization(matrix1, matrix2, 64);
            MatrixMultiplication multiplication = new MatrixMultiplication(matrix1, matrix2);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //Multiplication with thread
            var result1 = multiplication.DoMultiplicationWithThread();
            stopwatch.Stop();
            Console.WriteLine("Multiplication with thread :" + stopwatch.Elapsed.ToString());
            stopwatch.Restart();
            //Multiplication with threadpool
            var result2 = multiplication.DoMultiplicationWithThreadPool();
            stopwatch.Stop();
            Console.WriteLine("Multiplication with threadpool :" + stopwatch.Elapsed.ToString());
            stopwatch.Restart();
            //Multiplication with Task
            var result3 = await multiplication.DoMultiplicationWithTask();
            stopwatch.Stop();
            Console.WriteLine("Multiplication with Task :" + stopwatch.Elapsed.ToString());
            stopwatch.Restart();
            //Multiplication with parallel
            var result4 = multiplication.DoMultiplicationWithParallel();
            stopwatch.Stop();
            Console.WriteLine("Multiplication with parallel :" + stopwatch.Elapsed.ToString());
            Console.ReadLine();
         }
        static void initialization(double[,] matrix1, double[,] matrix2,int seed) 
        {
            Random random = new Random(seed);
            //Initialization
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix1.GetLength(1); j++)
                {
                    matrix1[i, j] = random.NextDouble() * 1000;
                }
            }
            for (int i = 0; i < matrix2.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    matrix2[i, j] = random.NextDouble() * 2000;
                }
            }
        }
    }
}
