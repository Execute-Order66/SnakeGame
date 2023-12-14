
using System.Collections.Generic;

namespace SnakeGame
{
    public class Direction
    {
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);

        public int RowOffSet { get; }
        public int ColOffSet { get; }

        private Direction(int rowOffSet, int colOffSet)
        {
            RowOffSet = rowOffSet;
            ColOffSet = colOffSet;
        }
        public Direction Opposite()
        {
            return new Direction(-RowOffSet, -ColOffSet);
        }

        public override bool Equals(object obj)
        {
            return obj is Direction direction &&
                   RowOffSet == direction.RowOffSet &&
                   ColOffSet == direction.ColOffSet;
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
}
