using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConcurrentOperation
{
    public class Coordination
    {
        public int Row {  get; set; }
        public int Column { get; set; }
        public double[,] Matrix1 { get; set; }
        public double[,] Matrix2 { get; set; }
    }
}
