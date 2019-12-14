using System;
using Xylit.Playground;
using Xylit.Playground.GameObjects;
using System.Threading;
using System.Diagnostics;

namespace Xylit
{
    class Game
    {
        static bool cursorVisible;
        public static bool CursorVisible
        {
            get { return cursorVisible; }
            set
            {
                cursorVisible = value;
                Console.CursorVisible = value;
            }
        }

        public static ConsoleKeyInfo GetKeyPress(bool showPressedKey)
        {
            return Console.ReadKey(!showPressedKey);
        }
        public static ConsoleKeyInfo GetKeyPress()
        {
            return Console.ReadKey(true);
        }

        public static void ReplaceInMap(Region region, GameObject obj1, GameObject obj2)
        {
            region.Replace(obj1, obj2);
        }

        public static void SetScreenUp()
        {
            Console.SetCursorPosition(0, 0);
        }

        public static void ResetScreen(int width, int height)
        {
            Console.SetCursorPosition(0, 0);

            Console.WindowHeight = height;
            Console.BufferHeight = height;
            Console.WindowWidth = width;
            Console.BufferWidth = width;
        }

        public static int Time { get { return (int)clock.ElapsedMilliseconds; } }

        static Stopwatch clock = new Stopwatch();

        public static void StartTimer()
        {
            clock.Start();
        }
        public static void StopTimer()
        {
            clock.Stop();
        }
    }
}
