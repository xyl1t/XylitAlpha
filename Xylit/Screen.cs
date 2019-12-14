using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xylit.UI
{
    enum ScreenState
    {
        Normal,
        Changed,
    }
    class Screen
    {
        //Screen instance;
        //public Screen GetInstance { get { return instance; } }

        ScreenState screenState;
        public ScreenState ScreenState { get { return screenState; } }
        public ProChar[,] screen;
        ProChar[,] originalScreen;
        ProChar[,] proposedScreen;

        ConsoleColor foreColor;
        ConsoleColor backColor;

        // MAKE SET:
        // if set than re draw in new color 
        public ConsoleColor ForeColor { get { return foreColor; } set { foreColor = value; } }
        public ConsoleColor BackColor { get { return backColor; } set { backColor = value; } }

        int width, height;
        int Width
        {
            get { return width; }
            set
            {
                width = value;
                Console.WindowWidth = width;
                Console.BufferWidth = width;
            }
        }
        int Height
        {
            get { return height; }
            set
            {
                height = value;
                Console.WindowHeight = height;
                Console.BufferHeight = height;
            }
        }

        public ProChar this[int x, int y]
        {
            get { return this.screen[x, y]; }
        }

        public Screen(int width, int height)
        {
            this.width = width;
            this.height = height;

            this.foreColor = ConsoleColor.White;
            this.backColor = ConsoleColor.Black;

            screen = new ProChar[width, height];
            proposedScreen = new ProChar[width, height];
            originalScreen = new ProChar[width, height];
            Copy(ref proposedScreen, ref screen);
            Copy(ref originalScreen, ref screen);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    screen[x, y] = new ProChar(' ', x, y, foreColor, backColor);

            screenState = ScreenState.Normal;
        }
        public Screen(int width, int height, ConsoleColor foreColor, ConsoleColor backColor)
        {
            this.width = width;
            this.height = height;

            this.foreColor = foreColor;
            this.backColor = backColor;

            screen = new ProChar[width, height];
            Copy(ref proposedScreen, ref screen);
            Copy(ref originalScreen, ref screen);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    screen[x, y] = new ProChar(' ', x, y, foreColor, backColor);

            screenState = ScreenState.Normal;
        }

        public void Up()
        {
            Console.SetCursorPosition(0, 0);
        }

        public void ResetScreen()
        {
            Console.SetCursorPosition(0, 0);

            Console.WindowHeight = height;
            Console.BufferHeight = height;
            Console.WindowWidth = width;
            Console.BufferWidth = width;

            Console.ResetColor();
        }
        public void ResetScreen(int x, int y)
        {
            Console.SetCursorPosition(0, 0);

            Console.WindowHeight = height;
            Console.BufferHeight = height;
            Console.WindowWidth = width;
            Console.BufferWidth = width;

            Console.ResetColor();
        }

        public void Draw()
        {
            Console.SetBufferSize(1000, 1000);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    //if (x == Console.WindowWidth - 1 && y == Console.WindowHeight - 1)

                    if (screenState == ScreenState.Normal)
                    {
                        if (screen[x, y].Character != '\0')
                            screen[x, y].Draw();
                    }
                    else if (screenState == ScreenState.Changed)
                    {
                        if (proposedScreen[x, y].Character != '\0')
                            proposedScreen[x, y].Draw();
                    }
                }
            }
            ResetScreen();
        }

        public void DrawArea(int x, int y, int width, int height)
        {
            string drawing = String.Empty;

                int tempX = Console.CursorLeft, tempY = Console.CursorTop;


                for (int fx = x; fx < width + x; fx++)
                    for (int fy = y; fy < height + y; fy++)
                        if (screenState == ScreenState.Normal)
                            screen[fx, fy].Draw();
                        else if (screenState == ScreenState.Changed)
                            proposedScreen[fx, fy].Draw();

                ResetScreen(tempX, tempY);
        }

        public void Set(ProChar[,] area, int x, int y)
        {
            Copy(ref originalScreen, ref screen);
            //originalScreen = screen;
            Copy(ref proposedScreen, ref screen);

            for (int fx = x; fx < area.GetUpperBound(0) + 1 + x; fx++)
            {
                for (int fy = y; fy < area.GetUpperBound(1) + 1 + y; fy++)
                {
                    proposedScreen[fx, fy] = area[fx - x, fy - y];
                }
            }

            screenState = ScreenState.Changed;
        }
        public void Set(ProChar[,] area)
        {
            if (area.GetUpperBound(0) > this.screen.GetUpperBound(0) ||
               area.GetUpperBound(1) > this.screen.GetUpperBound(1))
            {
                ProChar[,] Temp = screen;

                screen = new ProChar[area.GetUpperBound(0) +1, area.GetUpperBound(1) + 1];
                proposedScreen = new ProChar[area.GetUpperBound(0) +1, area.GetUpperBound(1) + 1];
                originalScreen = new ProChar[area.GetUpperBound(0) +1, area.GetUpperBound(1) + 1];

                width = area.GetUpperBound(0) +1;
                height = area.GetUpperBound(1) +1;
                Copy(ref screen, ref Temp);
            }

            Copy(ref originalScreen, ref screen);
            Copy(ref proposedScreen, ref area);
            //originalScreen = new ProChar[width, height];
            //proposedScreen = new ProChar[width, height];
            //originalScreen = screen;
            //proposedScreen = area;

            screenState = ScreenState.Changed;
        }
        public void Set(ProChar symbol)
        {
            Copy(ref originalScreen, ref screen);
            Copy(ref proposedScreen, ref screen);
            proposedScreen[symbol.X, symbol.Y] = symbol;

            screenState = ScreenState.Changed;
        }

        public void Set(string text, int x, int y)
        {

            for (int fx = x; fx < text.Length + x; fx++)
            {
                try
                {
                    Set(new ProChar(text[fx - x], fx, y, foreColor, backColor));
                    AcceptChanges();
                }
                catch (IndexOutOfRangeException)
                {
                    y++;
                    x -= fx;
                    fx = 0;
                    Set(new ProChar(text[fx - x], fx, y, foreColor, backColor));
                    AcceptChanges();
                }

                //for (int fy = y; fy < height; fy++)
                //{

                //}
            }

        }
        public void Set(string text, int x, int y, ConsoleColor foreColor, ConsoleColor backColor)
        {

            for (int fx = x; fx < text.Length + x; fx++)
            {
                try
                {
                    Set(new ProChar(text[fx - x], fx, y, foreColor, backColor));
                    AcceptChanges();
                }
                catch (IndexOutOfRangeException)
                {
                    y++;
                    x -= fx;
                    fx = 0;
                    Set(new ProChar(text[fx - x], fx, y, foreColor, backColor));
                    AcceptChanges();
                }

                //for (int fy = y; fy < height; fy++)
                //{

                //}
            }

            AcceptChanges();
        }


        public void AcceptChanges()
        {
            Copy(ref screen, ref proposedScreen);
            Copy(ref proposedScreen, ref screen);
            Copy(ref originalScreen, ref screen);

            screenState = ScreenState.Normal;
        }

        public void RejectChanges()
        {
            Copy(ref screen, ref originalScreen);
            Copy(ref proposedScreen, ref screen);
            Copy(ref originalScreen, ref screen);

            screenState = ScreenState.Normal;
        }

        void Copy(ref ProChar[,] target, ref ProChar[,] value)
        {
            int tempWidth = 0, tempHeight = 0;
            tempWidth = (target.GetUpperBound(0) < value.GetUpperBound(0)) ? target.GetUpperBound(0) + 1: value.GetUpperBound(0) +1;
            tempHeight = (target.GetUpperBound(1) < value.GetUpperBound(1)) ? target.GetUpperBound(1)  + 1: value.GetUpperBound(1) + 1;

            for (int x = 0; x < tempWidth; x++)
                for (int y = 0; y < tempHeight; y++)
                    target[x, y] = value[x, y];
        }
    }
}
