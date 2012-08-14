using System;
using CrashNet.Engine;

namespace CrashNet
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game game = new Game(608, 608, false))
            {
                game.Run();
            }
        }
    }
#endif
}

