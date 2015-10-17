using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTools
{
    public struct Position
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Position North()
        {
            return new Position(X,Y + 1, Z);
        }

        public Position NorthWest()
        {
            return new Position(X-1,Y+1,Z);
        }

        public Position NorthEast()
        {
            return new Position(X + 1, Y + 1, Z);
        }

        public Position South()
        {
            return new Position(X, Y - 1, Z);
        }

        public Position SouthWest()
        {
            return new Position(X-1,Y-1,Z);
        }

        public Position SouthEast()
        {
            return new Position(X+1,Y-1,Z);
        }

        public Position East()
        {
            return new Position(X+1,Y,Z);
        }

        public Position West()
        {
            return new Position(X-1,Y,Z);
        }

        public override string ToString()
        {
            return string.Format("{{x:{0},y:{1},z:{2}}}",X,Y,Z);
        }

        public static bool operator ==(Position a, Position b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }
    }
}
