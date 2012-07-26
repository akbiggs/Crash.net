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
            using (Game game = new Game(800, 600, false))
            {
                game.Run();
            }
        }
    }
#endif
}
