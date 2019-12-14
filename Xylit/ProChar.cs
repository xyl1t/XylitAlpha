using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xylit.UI
{
    struct ProChar
    {
        private object LockObject;
        public char Character;
        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;

        public int X, Y;

        public ProChar(char symbol) : this(symbol, Console.ForegroundColor, Console.BackgroundColor) { }
        public ProChar(char symbol, int x, int y) : this(symbol, x, y, ConsoleColor.White, ConsoleColor.Black) { }
        public ProChar(char symbol, ConsoleColor foregroundColor, ConsoleColor backgroundColor) : this(symbol, 0, 0, foregroundColor, backgroundColor) { }
        public ProChar(char symbol, int x, int y, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            LockObject = new object();
            this.Character = symbol;
            this.X = x;
            this.Y = y;
            this.ForeColor = foregroundColor;
            this.BackColor = backgroundColor; 
            
        }

        public void Draw()
        {
            Monitor.Enter("DRAW");
            try
            {
                Console.SetCursorPosition(X, Y);
                Console.ForegroundColor = ForeColor;
                Console.BackgroundColor = BackColor;
                Console.Write((char)Character);
            }
            finally { Monitor.Exit("DRAW"); }
        }

        public void Draw(int offsetX, int offsetY)
        {
            Monitor.Enter("DRAW");
            try
            {
                Console.SetCursorPosition(X + offsetX, Y + offsetY);
                Console.ForegroundColor = ForeColor;
                Console.BackgroundColor = BackColor;
                Console.Write(((char)Character));
            }
            finally { Monitor.Exit("DRAW"); }
        }

        public void CopyChar(ProChar source)
        {
            this = source;
        }
        public void CopySymbol(ProChar source)
        {
            this = new ProChar(source.Character, this.X, this.Y, this.ForeColor, this.BackColor);
        }

        public void NewLocation(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static ConsoleColor GetInvertedColor(ConsoleColor targetColor)
        {
            ConsoleColor invertedColor = targetColor;

            if (targetColor == ConsoleColor.Black)
                invertedColor = ConsoleColor.White;
            if (targetColor == ConsoleColor.White)
                invertedColor = ConsoleColor.Black;

            if (targetColor == ConsoleColor.Gray)
                invertedColor = ConsoleColor.DarkGray;
            if (targetColor == ConsoleColor.DarkGray)
                invertedColor = ConsoleColor.Gray;



            if (targetColor == ConsoleColor.Green)
                invertedColor = ConsoleColor.Magenta;
            if (targetColor == ConsoleColor.Magenta)
                invertedColor = ConsoleColor.Green;

            if (targetColor == ConsoleColor.Red)
                invertedColor = ConsoleColor.Cyan;
            if (targetColor == ConsoleColor.Cyan)
                invertedColor = ConsoleColor.Red;

            if (targetColor == ConsoleColor.Blue)
                invertedColor = ConsoleColor.Yellow;
            if (targetColor == ConsoleColor.Yellow)
                invertedColor = ConsoleColor.Blue;


            if (targetColor == ConsoleColor.DarkGreen)
                invertedColor = ConsoleColor.DarkMagenta;
            if (targetColor == ConsoleColor.DarkMagenta)
                invertedColor = ConsoleColor.DarkGreen;

            if (targetColor == ConsoleColor.DarkRed)
                invertedColor = ConsoleColor.DarkCyan;
            if (targetColor == ConsoleColor.DarkCyan)
                invertedColor = ConsoleColor.DarkRed;

            if (targetColor == ConsoleColor.DarkBlue)
                invertedColor = ConsoleColor.DarkYellow;
            if (targetColor == ConsoleColor.DarkYellow)
                invertedColor = ConsoleColor.DarkBlue;


            return invertedColor;
        }
    }
}
