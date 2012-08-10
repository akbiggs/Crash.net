using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrashNet.Worlds;

namespace CrashNet.Engine
{
    public static class RoomManager
    {
        static Dictionary<string, Tile[,]> tileMapDict = new Dictionary<string, Tile[,]>();

        static List<string> names = new List<string>
        {
            "partitioned.csv",
            "smiley.csv"
        };
    }
}
