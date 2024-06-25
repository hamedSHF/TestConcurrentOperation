using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConcurrentOperation
{
    public class MatrixMultiplication
    {
        private double[,] matrix1;
        private double[,] matrix2;
        private double[,] matrix3;
        private Semaphore semaphore = new Semaphore(1, 1);
        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        public MatrixMultiplication(double[,] matrix1, double[,] matrix2)
        {
            this.matrix1 = matrix1;
            this.matrix2 = matrix2;
            this.matrix3 = new double[matrix1.GetLength(0), matrix2.GetLength(1)];
            if(matrix1.GetLength(1)  != matrix2.GetLength(0))
            {
                throw new Exception("Not acceptable matrices.");
            }
        }
        public double[,] DoMultiplicationWithThread()
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    int tempi = i;
                    int tempj = j;
                    Thread thread = new Thread(() => multiplicate(new Coordination { Row = tempi,Column = tempj},matrix1,matrix2));
                    thread.Start();
                    threads.Add(thread);
                }
            }
            foreach(Thread thread in threads)
                thread.Join();
            return matrix3;
        }
        public async Task<double[,]> DoMultiplicationWithTask()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    int tempi = i;
                    int tempj = j;
                    Task task = Task.Run(() => multiplicate(new Coordination { Row = tempi, Column = tempj }, matrix1, matrix2));
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
            return matrix3;
        }
        public double[,] DoMultiplicationWithThreadPool()
        {
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    int tempi = i;
                    int tempj = j;
                    ThreadPool.QueueUserWorkItem((coordincation) => multiplicateForPool(new Coordination { Row = tempi, Column = tempj })); 
                }
            }
            resetEvent.WaitOne(-1);
            return matrix3;
        }
        public double[,] DoMultiplicationWithParallel()
        {
            Action[] actions = new Action[matrix1.GetLength(0)*matrix2.GetLength(1)];
            int counter = 0;
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    int tempi = i;
                    int tempj = j;
                    actions[counter] =() => multiplicateForPool(new Coordination { Row = tempi, Column = tempj });
                    counter++;
                }
            }
            Parallel.Invoke(actions);
            resetEvent.WaitOne(-1);
            return matrix3;
        }
        private void multiplicate(Coordination coordination, double[,] matrix1, double[,] matrix2)
        {
            semaphore.WaitOne();
            for (int i = 0; i < GetRow(coordination.Row, matrix1).Length; i++)
            {
                matrix3[coordination.Row, coordination.Column] += matrix1[coordination.Row, i] * matrix2[i, coordination.Column]; 
            }
            semaphore.Release();
        }
        private void multiplicateForPool(Coordination coordination)
        {
            semaphore.WaitOne();
            for (int i = 0; i < GetRow(coordination.Row, matrix1).Length; i++)
            {
                matrix3[coordination.Row, coordination.Column] += matrix1[coordination.Row, i] * matrix2[i, coordination.Column];
            }
            if(coordination.Row == matrix1.GetLength(0) - 1 && coordination.Column == matrix2.GetLength(1) - 1)
            {
                resetEvent.Set();
            }
            semaphore.Release();
        }
        public double[] GetRow(int row, double[,] matrix)
        {
            double[] rows = new double[matrix.GetLength(1)];
            for(int i =0;i < matrix.GetLength(1);i++)
                rows[i] = matrix[row,i];
            return rows;
        }
        public double[] GetColumn(int col, double[,] matrix)
        {
            double[] columns = new double[matrix.GetLength(0)];
            for(int i =0;i < matrix.GetLength(0);i++)
                columns[i] = matrix[i,col];
            return columns;
        }
    }
}
