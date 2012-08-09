using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrashNet.Worlds
{
    static class WorldGenerator
    {
        public static World Generate(WorldNumber number)
        {
            switch (number)
            {
                case WorldNumber.One:
                default:
                    return new World(5, 5);
                case WorldNumber.Two:
                    return new World(7, 7);
                case WorldNumber.Three:
                    return new World(9, 9);
                case WorldNumber.Four:
                    return new World(11, 11);
            }
        }
    }
}
