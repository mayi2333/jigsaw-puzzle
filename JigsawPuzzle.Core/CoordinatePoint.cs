using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JigsawPuzzle.Core
{
    public struct CoordinatePoint
    {
        public CoordinatePoint(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
    }
}
