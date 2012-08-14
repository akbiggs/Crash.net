using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrashNet.Engine
{
    public enum Direction
    {
        North,
        NorthWest,
        West,
        SouthWest,
        South,
        SouthEast,
        East,
        NorthEast,
        None
    }

    public static class DirectionOperations
    {

        const double PI = MathHelper.Pi;
        const double PI_OVER_TWO = MathHelper.PiOver2;
        const double THREE_PI_OVER_TWO = 1.5 * PI;
        const double TWO_PI = MathHelper.TwoPi;
        //diagonals
        const double PI_OVER_FOUR = MathHelper.PiOver4;
        const double THREE_PI_OVER_FOUR = 0.75 * PI;
        const double FIVE_PI_OVER_FOUR = 1.25 * PI;
        const double SEVEN_PI_OVER_FOUR = 1.75 * PI;

        public static Vector2 ToVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                default:
                    return new Vector2(0, 0);
                case Direction.North:
                    return new Vector2(0, -1);
                case Direction.NorthWest:
                    return new Vector2(-1, -1);
                case Direction.West:
                    return new Vector2(-1, 0);
                case Direction.SouthWest:
                    return new Vector2(-1, 1);
                case Direction.South:
                    return new Vector2(0, 1);
                case Direction.SouthEast:
                    return new Vector2(1, 1);
                case Direction.East:
                    return new Vector2(1, 0);
                case Direction.NorthEast:
                    return new Vector2(1, -1);
            }
        }

        /// <summary>
        /// Converts the given direction into radians.
        /// </summary>
        /// <param name="direction">A direction.</param>
        /// <returns>The angle of rotation of the direction, in radians.</returns>
        public static double ToRadians(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                default:
                    return 0;
                case Direction.NorthWest:
                    return SEVEN_PI_OVER_FOUR;
                case Direction.West:
                    return THREE_PI_OVER_TWO;
                case Direction.SouthWest:
                    return FIVE_PI_OVER_FOUR;
                case Direction.South:
                    return PI;
                case Direction.SouthEast:
                    return THREE_PI_OVER_FOUR;
                case Direction.East:
                    return PI_OVER_TWO;
                case Direction.NorthEast:
                    return PI_OVER_FOUR;
            }
        }

        /// <summary>
        /// Whether or not the given direction is horizontal.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <returns>True if the direction is horizontal, false otherwise.</returns>
        public static bool IsHorizontal(Direction direction)
        {
            return ToVector(direction).X != 0;
        }

        /// <summary>
        /// Whether or not the given direction is vertical.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <returns>True if the direction is vertical, false otherwise.</returns>
        public static bool IsVertical(Direction direction)
        {
            return ToVector(direction).Y != 0;
        }

        /// <summary>
        /// Combines the list of given directions into a single direction.
        /// </summary>
        /// <param name="directions">The directions to combine.</param>
        /// <returns>A single direction derived from the given directions.</returns>
        public static Direction Combine(List<Direction> directions)
        {
            // remove opposing directions
            if (directions.Contains(Direction.West) && directions.Contains(Direction.East))
            {
                directions.Remove(Direction.West);
                directions.Remove(Direction.East);
            }
            if (directions.Contains(Direction.North) && directions.Contains(Direction.South))
            {
                directions.Remove(Direction.North);
                directions.Remove(Direction.South);
            }

            if (directions.Count == 0)
                return Direction.None;
            else if (directions.Count == 1)
                return directions[0];
            else if (directions.Contains(Direction.North) && directions.Contains(Direction.West))
                return Direction.NorthWest;
            else if (directions.Contains(Direction.North) && directions.Contains(Direction.East))
                return Direction.NorthEast;
            else if (directions.Contains(Direction.South) && directions.Contains(Direction.West))
                return Direction.SouthWest;
            else
                return Direction.SouthEast;
        }
    }
}
